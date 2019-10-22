﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.DAO;
using System.IO;
using System.Net;
using SnmpSharpNet;
using System.Collections;
using SNMPDiscovery.Model.Helpers;

namespace SNMPDiscovery.Model.Services
{
    public class SNMPModel : ISNMPModelDTO
    {
        private const int DefaultPort = 161;
        private const int DefaultTimeout = 1000;
        private const int DefaultRetries = 1;
        private IList<IObserver<ISNMPModelDTO>> _snmpModelObservers;

        public IDictionary<string, ISNMPDeviceSettingDTO> SNMPDeviceSettings { get; set; }
        public IDictionary<string, ISNMPDeviceDataDTO> SNMPDeviceData { get; set; }
        public IDictionary<string, ISNMPProcessedValueDTO> SNMPGlobalProcessedData { get; set; }
        
        public CustomPair<Type, object> ChangedObject { get; set; }

        #region Interface Implementations

        public void NotifyError(Exception error)
        {
            foreach (IObserver<ISNMPModelDTO> observer in _snmpModelObservers)
            {
                observer.OnError(error);
            }
        }

        public void NotifyChanges()
        {
            foreach (IObserver<ISNMPModelDTO> observer in _snmpModelObservers)
            {
                observer.OnNext(this);
            }
        }

        public IDisposable Subscribe(IObserver<ISNMPModelDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_snmpModelObservers.Contains(observer))
            {
                _snmpModelObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<ISNMPModelDTO>(_snmpModelObservers, observer);
        }

        public ISNMPDeviceSettingDTO BuildSNMPSetting(string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            //Lazy initialization
            if (SNMPDeviceSettings == null)
            {
                SNMPDeviceSettings = new Dictionary<string, ISNMPDeviceSettingDTO>();
            }

            ISNMPDeviceSettingDTO setting = new SNMPDeviceSettingDTO(ID, initialIPAndMask, finalIPAndMask, SNMPUser, ChangeTrackerHandler);
            SNMPDeviceSettings.Add(ID, setting);

            return setting;
        }

        public ISNMPDeviceDataDTO BuildSNMPDevice(string targetIPAndMask)
        {
            //Lazy initialization
            if (SNMPDeviceData == null)
            {
                SNMPDeviceData = new Dictionary<string, ISNMPDeviceDataDTO>();
            }

            ISNMPDeviceDataDTO device = new SNMPDeviceDataDTO(ModelHelper.ExtractIPAddress(targetIPAndMask), ModelHelper.ExtractNetworkMask(targetIPAndMask), ChangeTrackerHandler);
            SNMPDeviceData.Add(targetIPAndMask, device);

            return device;
        }

        public ISNMPDeviceDataDTO BuildSNMPDevice(int targetIP, int targetMask)
        {
            //Lazy initialization
            if (SNMPDeviceData == null)
            {
                SNMPDeviceData = new Dictionary<string, ISNMPDeviceDataDTO>();
            }

            ISNMPDeviceDataDTO device = new SNMPDeviceDataDTO(targetIP, targetMask, ChangeTrackerHandler);
            SNMPDeviceData.Add(new IPAddress(targetIP).ToString(), device);

            return device;
        }

        public ISNMPDeviceDataDTO BuildSNMPDevice(IPAddress targetIP, int targetMask)
        {
            //Lazy initialization
            if (SNMPDeviceData == null)
            {
                SNMPDeviceData = new Dictionary<string, ISNMPDeviceDataDTO>();
            }

            ISNMPDeviceDataDTO device = new SNMPDeviceDataDTO(targetIP, targetMask, ChangeTrackerHandler);
            SNMPDeviceData.Add(targetIP.ToString(), device);

            return device;
        }

        public ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data)
        {
            //Lazy initialization
            if (SNMPGlobalProcessedData == null)
            {
                SNMPGlobalProcessedData = new Dictionary<string, ISNMPProcessedValueDTO>();
            }

            ISNMPProcessedValueDTO ProcessedValue = new SNMPProcessedValueDTO(DataType, Data);
            SNMPGlobalProcessedData.Add(DataType.Name, ProcessedValue);
            //We don't trigger OnChange because we only set the poiner, information is still not filled.

            return ProcessedValue;
        }

        public void StartDiscovery()
        {
            //Iterate on all settings
            foreach (ISNMPDeviceSettingDTO SNMPSettingEntry in SNMPDeviceSettings.Values)
            {
                SNMPWalkByIP(SNMPSettingEntry);
            }
        }

        public void RunProcesses()
        {
            IEnumerable<ISNMPProcessStrategy> processes = SNMPDeviceSettings.SelectMany(x => x.Value.Processes.Values);

            foreach (ISNMPProcessStrategy alg in processes)
            {
                //Validate proper inputs
                alg.ValidateInput(this);
                //Run process

                alg.Run(this);
                //Notify alg done
            }
        }

        public void SaveData()
        {
            //Validate exists DataDestination Settings
            //Invoke DAOData exporter realization
            //Get & serialize object to destination
        }

        #endregion

        #region Nested Object Change Handlers

        public void ChangeTrackerHandler(object obj, Type type)
        {
            ChangedObject.First = type;
            ChangedObject.Second = obj;

            NotifyChanges();
        }

        #endregion

        #region Private Methods

        private void SNMPWalkByIP(ISNMPDeviceSettingDTO SNMPSettingEntry)
        {
            IList<IPAddress> IPinventory;
            OctetString Community;
            AgentParameters AgParam;
            Pdu pdu;

            //Compute full list of devices
            IPinventory = ModelHelper.GenerateHostList(SNMPSettingEntry.InitialIP, SNMPSettingEntry.FinalIP, SNMPSettingEntry.NetworkMask);
            //Discard unavailable devices for SNMP

            Community = new OctetString(SNMPSettingEntry.CommunityString);

            AgParam = new AgentParameters(Community);
            AgParam.Version = SnmpVersion.Ver2; // Set SNMP version to 2 (GET-BULK only works with SNMP ver 2 and 3)

            pdu = new Pdu(PduType.GetBulk);
            pdu.NonRepeaters = 0; //NonRepeaters tells how many OID asociated values (leafs of this object) get. 0 is all
            pdu.MaxRepetitions = 5; // MaxRepetitions tells the agent how many Oid/Value pairs to return in the response packet.
            pdu.RequestId = 1;

            using (UdpTarget UDPtarget = new UdpTarget(new IPAddress(0), DefaultPort, DefaultTimeout, DefaultRetries))
            {
                foreach (IPAddress target in IPinventory)
                {
                    ISNMPDeviceDataDTO device = BuildSNMPDevice(target, SNMPSettingEntry.NetworkMask);
                    UDPtarget.Address = device.TargetIP;
                    SNMPWalkByOIDSetting(UDPtarget, pdu, AgParam, SNMPSettingEntry, device);
                }
            }
        }

        private void SNMPWalkByOIDSetting(UdpTarget UDPtarget, Pdu pdu, AgentParameters param, ISNMPDeviceSettingDTO SNMPSettingEntry, ISNMPDeviceDataDTO SNMPDeviceData)
        {
            //Get all OID requested for all processing algorithms
            IEnumerable<IOIDSettingDTO> OIDSettingCollection = SNMPSettingEntry.OIDSettings.Values;
            Oid indexOid = new Oid();
            Oid finalOid = new Oid();

            foreach (IOIDSettingDTO OIDSetting in OIDSettingCollection)
            {
                SNMPRunAgent(UDPtarget, pdu, param, OIDSetting, SNMPDeviceData);
            }
        }

        private void SNMPRunAgent(UdpTarget UDPtarget, Pdu pdu, AgentParameters param, IOIDSettingDTO OIDSetting, ISNMPDeviceDataDTO SNMPDeviceData)
        {
            bool nextEntry = true;
            SnmpV2Packet Result;
            Oid indexOid = new Oid(OIDSetting.InitialOID); //Walk control
            Oid finalOid = new Oid(OIDSetting.FinalOID);

            while (nextEntry)
            {
                pdu.VbList.Add(indexOid); //Add starting OID for request

                try
                {
                    Result = (SnmpV2Packet)UDPtarget.Request(pdu, param);
                    nextEntry = SNMPDecodeData(Result, indexOid, finalOid, OIDSetting.InclusiveInterval, SNMPDeviceData);
                }
                catch (Exception e)
                {
                    NotifyError(e);
                    nextEntry = false;
                }

                //Prepare PDU object for iteration. Otherwise, wipe contents of pdu
                pdu.RequestId++;
                pdu.VbList.Clear();
            }
        }

        private bool SNMPDecodeData(SnmpV2Packet Result, Oid indexOid, Oid finalOid, bool InclusiveInterval, ISNMPDeviceDataDTO SNMPDeviceData)
        {
            bool nextEntry = true;

            if (Result != null && Result.Pdu.ErrorStatus == 0)
            {
                // Walk through returned variable bindings
                foreach (Vb ResBinding in Result.Pdu.VbList)
                {
                    // Check that retrieved Oid is higher than the limit or is the last block of leafs
                    if (ResBinding.Oid < finalOid || finalOid.IsRootOf(ResBinding.Oid) && InclusiveInterval)
                    {
                        ISNMPRawEntryDTO SNMPRawData = SNMPDeviceData.BuildSNMPRawEntry(ResBinding.Oid.ToString(), ResBinding.Value.ToString(), (EnumSNMPOIDType)Enum.Parse(typeof(EnumSNMPOIDType), SnmpConstants.GetTypeName(ResBinding.Value.Type)));

                        //Check if we have already drilled down all contents
                        if (ResBinding.Value.Type != SnmpConstants.SMI_ENDOFMIBVIEW)
                        {
                            indexOid.Set(ResBinding.Oid); //If we use = operator, we are changing references! In that case, ref keyword is mandatory
                            nextEntry = true;
                        }
                    }
                    else
                    {
                        nextEntry = false;
                        break;
                    }
                }
            }
            else
            {
                //Console.WriteLine("Error in SNMP reply. Error {0} index {1}", Result.Pdu.ErrorStatus, Result.Pdu.ErrorIndex);
            }

            return nextEntry;
        }

        #endregion

        #region Constructors

        public SNMPModel()
        {
            _snmpModelObservers = new List<IObserver<ISNMPModelDTO>>();
            ChangedObject = new CustomPair<Type, object>();
        }

        #endregion

    }
}
