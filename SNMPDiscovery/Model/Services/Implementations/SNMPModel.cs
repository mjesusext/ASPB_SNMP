﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using System.IO;
using System.Net;
using SnmpSharpNet;
using SNMPDiscovery.Model.Helpers;
using System.Collections.Concurrent;

namespace SNMPDiscovery.Model.Services
{
    public class SNMPModel : ISNMPModelDTO
    {
        #region Private fields

        private const int DefaultPort = 161;
        private const int DefaultTimeout = 2000;
        private const int DefaultRetries = 2;
        private IList<IObserver<ISNMPModelDTO>> _snmpModelObservers;

        #endregion

        #region Public properties

        public IDictionary<string, ISNMPDeviceSettingDTO> DeviceSettings { get; set; }
        public IDictionary<EnumProcessingType, ISNMPProcessStrategy> Processes { get; set; }
        public IDictionary<string, ISNMPDeviceDataDTO> DeviceData { get; set; }
        public IDictionary<string, ISNMPProcessedValueDTO> GlobalProcessedData { get; set; }
        public IDictionary<string, string> ARPTable { get; set; }
        public CustomPair<Type, object> ChangedObject { get; set; }
        
        #endregion

        #region Nested Object Change Handlers

        private void ChangeTrackerHandler(object obj, Type type)
        {
            ChangedObject.First = type;
            ChangedObject.Second = obj;

            NotifyChanges();
        }

        #endregion

        #region Observer Implementations

        public void NotifyError(Exception error)
        {
            if(_snmpModelObservers == null)
            {
                return;
            }

            foreach (IObserver<ISNMPModelDTO> observer in _snmpModelObservers)
            {
                observer.OnError(error);
            }
        }

        public void NotifyChanges()
        {
            if(_snmpModelObservers == null)
            {
                return;
            }

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

        #endregion

        #region Data methods

        public ISNMPDeviceSettingDTO BuildSNMPSetting(string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            ISNMPDeviceSettingDTO setting = new SNMPDeviceSettingDTO(ID, initialIPAndMask, finalIPAndMask, SNMPUser, ChangeTrackerHandler);
            DeviceSettings.Add(ID, setting);

            return setting;
        }

        public ISNMPDeviceSettingDTO EditSNMPSetting(string oldID, string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            ISNMPDeviceSettingDTO setting;

            //Validation
            if (DeviceSettings == null || !DeviceSettings.ContainsKey(oldID))
            {
                return null;
            }

            //Changing values of reference does not interfere on process targets. They keep the track of objects
            setting = DeviceSettings[oldID];
            setting.EditDeviceSetting(ID, initialIPAndMask, finalIPAndMask, SNMPUser);
            
            if(oldID != ID)
            {
                DeviceSettings.Remove(oldID);
                DeviceSettings.Add(ID, setting);
            }

            return setting;
        }

        public void DeleteSNMPSetting(string ID)
        {
            ISNMPDeviceSettingDTO setting;

            //Validation
            if (DeviceSettings == null || !DeviceSettings.ContainsKey(ID))
            {
                return;
            }

            setting = DeviceSettings[ID];

            //Drop references on process if exists
            if(Processes?.Values != null)
            {
                foreach (ISNMPProcessStrategy process in Processes.Values)
                {
                    process.TargetDeviceSettings.Remove(setting);
                }
            }

            //Drop main container
            DeviceSettings.Remove(ID);
        }

        public ISNMPDeviceDataDTO BuildSNMPDevice(IPAddress targetIP, int targetMask)
        {
            ISNMPDeviceDataDTO device = new SNMPDeviceDataDTO(targetIP, targetMask, ChangeTrackerHandler);
            DeviceData.Add(targetIP.ToString(), device);

            return device;
        }

        public ISNMPProcessStrategy BuildProcess(string SettingID, EnumProcessingType ProcessType)
        {
            ISNMPProcessStrategy ProcessProfile = null;
            ISNMPDeviceSettingDTO DeviceProfile = DeviceSettings?[SettingID];

            switch (ProcessType)
            {
                case EnumProcessingType.None:
                    break;
                case EnumProcessingType.TopologyDiscovery:

                    //Get existing strategy
                    if (!Processes.ContainsKey(ProcessType))
                    {
                        ProcessProfile = new TopologyBuilderStrategy(this, ChangeTrackerHandler);
                        Processes.Add(ProcessType, ProcessProfile);
                    }
                    else
                    {
                        ProcessProfile = Processes[ProcessType];
                    }

                    //Add device setting if found
                    if(DeviceProfile != null)
                    {
                        ProcessProfile.TargetDeviceSettings.Add(DeviceProfile);
                    }

                    ChangeTrackerHandler(ProcessProfile, typeof(ISNMPProcessStrategy));

                    break;
                case EnumProcessingType.PrinterConsumption:
                    break;
                default:
                    break;
            }

            return ProcessProfile;
        }

        public ISNMPProcessStrategy EditProcess(EnumProcessingType PreviousProcessType, EnumProcessingType NewProcessType)
        {
            ISNMPProcessStrategy ProcessProfile = null;

            //Validation
            if (Processes == null && NewProcessType == EnumProcessingType.None)
            {
                return null;
            }

            switch (NewProcessType)
            {
                case EnumProcessingType.None:
                    break;
                case EnumProcessingType.TopologyDiscovery:
              
                    if (!Processes.ContainsKey(NewProcessType))
                    {
                        ProcessProfile = new TopologyBuilderStrategy(this, ChangeTrackerHandler);
                        ProcessProfile.TargetDeviceSettings = Processes[PreviousProcessType].TargetDeviceSettings;

                        Processes.Add(NewProcessType, ProcessProfile);
                    }
                    else
                    {
                        ProcessProfile = Processes[NewProcessType];

                        //MJE - Pending of double-checking
                        ((List<ISNMPDeviceSettingDTO>)ProcessProfile.TargetDeviceSettings).AddRange(Processes[PreviousProcessType].TargetDeviceSettings);
                        ProcessProfile.TargetDeviceSettings = ProcessProfile.TargetDeviceSettings.Distinct().ToList();
                    }

                    Processes.Remove(PreviousProcessType);

                    break;
                case EnumProcessingType.PrinterConsumption:
                    break;
                default:
                    break;
            }

            return ProcessProfile;
        }

        public void DeleteProcess(EnumProcessingType ProcessType)
        {
            //Validation
            if(ProcessType == EnumProcessingType.None || !Processes.ContainsKey(ProcessType))
            {
                return;
            }

            Processes.Remove(ProcessType);
        }

        public ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data)
        {
            ISNMPProcessedValueDTO ProcessedValue = new SNMPProcessedValueDTO(DataType, Data);
            GlobalProcessedData.Add(DataType.Name, ProcessedValue);
            //We don't trigger OnChange because we only set the poiner, information is still not filled.

            return ProcessedValue;
        }

        #endregion

        #region Commands

        public void StartDiscovery()
        {
            //Validate values
            if (DeviceSettings?.Values == null)
            {
                return;
            }

            //Feed MAC-IP mappings previously
            //GetIPMACMappingsOnARP();
            GetIPMACMappingsOnDHCP();

            //Iterate on all settings
            foreach (ISNMPDeviceSettingDTO SNMPSettingEntry in DeviceSettings.Values)
            {
                SNMPWalkByIP(SNMPSettingEntry);
            }
        }

        public void RunProcesses()
        {
            //Validate values
            if (Processes?.Values == null)
            {
                return;
            }

            foreach (ISNMPProcessStrategy alg in Processes.Values)
            {
                alg.Run();
            }
        }

        public void SaveData()
        {
            //Validate exists DataDestination Settings
            //Invoke DAOData exporter realization
            //Get & serialize object to destination
        }

        #endregion

        #region SNMP Client Methods

        private void SNMPWalkByIP(ISNMPDeviceSettingDTO SNMPSettingEntry)
        {
            IList<IPAddress> IPinventory;
            OctetString Community;

            //Compute full list of devices that have responded to ARP request
            IPinventory = ModelHelper.GenerateHostList(SNMPSettingEntry.InitialIP, SNMPSettingEntry.FinalIP, SNMPSettingEntry.NetworkMask, ARPTable.Values.ToList());

            Community = new OctetString(SNMPSettingEntry.CommunityString);

            foreach (IPAddress target in IPinventory)
            {
                ISNMPDeviceDataDTO device = BuildSNMPDevice(target, SNMPSettingEntry.NetworkMask);

                try
                {
                    SNMPWalkByOIDSetting(Community, SNMPSettingEntry, device);
                }
                catch (SnmpException e)
                {
                    NotifyError(e);

                    //Device entry not containing full info for processing
                    DeviceData.Remove(target.ToString());
                    continue;
                }
            }
        }

        private void SNMPWalkByOIDSetting(OctetString Community, ISNMPDeviceSettingDTO SNMPSettingEntry, ISNMPDeviceDataDTO SNMPDeviceData)
        {
            //Get all OID requested for all processing algorithms
            IEnumerable<IOIDSettingDTO> OIDSettingCollection = Processes.Values.SelectMany(x => x.OIDSettings.Values);

            foreach (IOIDSettingDTO OIDSetting in OIDSettingCollection)
            {
                try
                {
                    SNMPRunAgent(Community, OIDSetting, SNMPDeviceData);
                }
                catch //(SnmpException e)
                {
                    throw;
                }
            }
        }

        private void SNMPRunAgent(OctetString Community, IOIDSettingDTO OIDSetting, ISNMPDeviceDataDTO SNMPDeviceData)
        {
            bool nextEntry = true;
            AgentParameters AgParam;
            Pdu pdu;
            SnmpV2Packet Result;
            Oid indexOid = new Oid(OIDSetting.InitialOID); //Walk control
            Oid finalOid = new Oid(OIDSetting.FinalOID);

            AgParam = new AgentParameters(Community);
            AgParam.Version = SnmpVersion.Ver2; // Set SNMP version to 2 (GET-BULK only works with SNMP ver 2 and 3)

            pdu = new Pdu(PduType.GetBulk);
            pdu.NonRepeaters = 0; //NonRepeaters tells how many OID asociated values (leafs of this object) get. 0 is all
            pdu.MaxRepetitions = 5; // MaxRepetitions tells the agent how many Oid/Value pairs to return in the response packet.
            pdu.RequestId = 1;

            using (UdpTarget UDPtarget = new UdpTarget(SNMPDeviceData.TargetIP, DefaultPort, DefaultTimeout, DefaultRetries))
            {
                while (nextEntry)
                {
                    pdu.VbList.Add(indexOid); //Add starting OID for request

                    try
                    {
                        Result = (SnmpV2Packet)UDPtarget.Request(pdu, AgParam);
                        nextEntry = SNMPDecodeData(Result, indexOid, finalOid, OIDSetting.InclusiveInterval, SNMPDeviceData);
                    }
                    catch //(SnmpException e)
                    {
                        throw;
                    }
                    finally
                    {
                        //Prepare PDU object for iteration. Otherwise, wipe contents of pdu
                        pdu.RequestId++;
                        pdu.VbList.Clear();
                    }
                }
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
                        //Check OID Value Type. If unknown we break loop and storage
                        EnumSNMPOIDType OIDType = (EnumSNMPOIDType)Enum.Parse(typeof(EnumSNMPOIDType), SnmpConstants.GetTypeName(ResBinding.Value.Type));

                        if(OIDType != EnumSNMPOIDType.Unknown)
                        {
                            ISNMPRawEntryDTO SNMPRawData = SNMPDeviceData.BuildSNMPRawEntry(ResBinding.Oid.ToString(), ResBinding.Value.ToString(), OIDType);
                        }
                        else
                        {
                            nextEntry = false;
                            break;
                        }

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

        #region ARP - DHCP Management

        private void GetIPMACMappingsOnARP()
        {
            List<IPAddress> IPinventory = new List<IPAddress>();

            foreach (ISNMPDeviceSettingDTO DeviceDef in DeviceSettings.Values)
            {
                IPinventory.AddRange(ModelHelper.GenerateHostList(DeviceDef.InitialIP, DeviceDef.FinalIP, DeviceDef.NetworkMask));
            }

            Parallel.ForEach(IPinventory.Select(x => x.ToString()), ARPMACsearch);
        }

        private void ARPMACsearch(string iptarget)
        {
            string MACaddr = ModelHelper.SendARPRequest(iptarget);

            lock (ARPTable)
            {
                if (!string.IsNullOrWhiteSpace(MACaddr) && !ARPTable.ContainsKey(MACaddr))
                {
                    ARPTable.Add(MACaddr, iptarget);
                }
            }
        }

        private void GetIPMACMappingsOnDHCP()
        {
            ModelHelper.GetDHCPleases(new List<string>() { "192.168.1.67", "192.1.1.74" }, ARPTable);
        }

        #endregion

        #region Constructors

        public SNMPModel()
        {
            _snmpModelObservers = new List<IObserver<ISNMPModelDTO>>();
            ChangedObject = new CustomPair<Type, object>();

            DeviceSettings = new Dictionary<string, ISNMPDeviceSettingDTO>();
            Processes = new Dictionary<EnumProcessingType, ISNMPProcessStrategy>();
            DeviceData = new Dictionary<string, ISNMPDeviceDataDTO>();
            GlobalProcessedData = new Dictionary<string, ISNMPProcessedValueDTO>();
            ARPTable = new Dictionary<string, string>();

        }

        #endregion
    }
}
