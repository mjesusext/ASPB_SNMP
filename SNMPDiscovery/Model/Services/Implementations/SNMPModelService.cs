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

namespace SNMPDiscovery.Model.Services
{
    public class SNMPModelService : ISNMPModelService
    {
        private const int DefaultPort = 161;
        private const int DefaultTimeout = 1000;
        private const int DefaultRetries = 1;

        private IList<IObserver<ISNMPModelDTO>> _modelObservers { get; set; }
        private ISNMPModelDTO _model { get; set; }

        public SNMPModelService()
        {
            _modelObservers = new List<IObserver<ISNMPModelDTO>>();
            _model = new SNMPModel();

            //Mock for redirecting console to file
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./Redirect.txt", FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);

            //Mock methods for development
            MockGetSettings();
            StartDiscovery();
            RunProcesses();

            //Mock for undoing things
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
        }

        #region Observable implementation

        public IDisposable Subscribe(IObserver<ISNMPModelDTO> observer)
        {
            // Check whether observer is already registered. If not, add it
            if (!_modelObservers.Contains(observer))
            {
                _modelObservers.Add(observer);
                // Provide observer with existing data. --> Iterate + OnNext()

            }
            return new SNMPObservableUnsubscriber<ISNMPModelDTO>(_modelObservers, observer);
        }

        #endregion

        #region Commands

        public void StartDiscovery()
        {
            //Iterate on all settings
            foreach (ISNMPSettingDTO SNMPSettingEntry in _model.SNMPSettings.Values)
            {
                SNMPWalkByIP(SNMPSettingEntry);
            }
        }

        public void RunProcesses()
        {
            IEnumerable<ISNMPProcessStrategy> processes = _model.SNMPSettings.SelectMany(x => x.Value.ProcessingProfiles).Select(x => x.Value.Process);

            foreach (ISNMPProcessStrategy alg in processes)
            {
                //Notify initialization
                Console.WriteLine($"Starting algorithm {alg.ID}");

                //Validate proper inputs
                alg.ValidateInput(_model.SNMPData);
                //Run process

                alg.Run(_model.SNMPData);
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

        #region Private Methods

        private void SNMPWalkByIP(ISNMPSettingDTO SNMPSettingEntry)
        {
            //Initialize enumerator, converting from little endian to big endian
            int LowerIPboundSNMP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(SNMPSettingEntry.InitialIP.GetAddressBytes(), 0));
            int UpperIPboundSNMP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(SNMPSettingEntry.FinalIP.GetAddressBytes(), 0));
            //Exclude broadcast and network --> Take into account net masks...

            OctetString community = new OctetString(SNMPSettingEntry.CommunityString);

            AgentParameters param = new AgentParameters(community);
            param.Version = SnmpVersion.Ver2; // Set SNMP version to 2 (GET-BULK only works with SNMP ver 2 and 3)

            Pdu pdu = new Pdu(PduType.GetBulk);
            pdu.NonRepeaters = 0; //NonRepeaters tells how many OID asociated values (leafs of this object) get. 0 is all
            pdu.MaxRepetitions = 5; // MaxRepetitions tells the agent how many Oid/Value pairs to return in the response packet.
            pdu.RequestId = 1;
            
            using (UdpTarget UDPtarget = new UdpTarget(new IPAddress(0), DefaultPort, DefaultTimeout, DefaultRetries))
            {
                for (int i = LowerIPboundSNMP; i <= UpperIPboundSNMP; i++)
                {
                    ISNMPDeviceDTO device = _model.BuildSNMPDevice(IPAddress.NetworkToHostOrder(i));
                    UDPtarget.Address = device.TargetIP;
                    SNMPWalkByOIDSetting(UDPtarget, pdu, param, SNMPSettingEntry, device);
                }
            }

            //Test
            Console.WriteLine("Cantidad de entradas: {0}", _model.SNMPData.Values.FirstOrDefault().SNMPRawDataEntries.Count);
        }

        private void SNMPWalkByOIDSetting(UdpTarget UDPtarget, Pdu pdu, AgentParameters param, ISNMPSettingDTO SNMPSettingEntry, ISNMPDeviceDTO SNMPDeviceData)
        {
            //Get all OID requested for all processing algorithms
            IEnumerable<IOIDSettingDTO> OIDSettingCollection = SNMPSettingEntry.ProcessingProfiles.Values.SelectMany(x => x.OIDSettingsCollection.Values);
            Oid indexOid = new Oid(); 
            Oid finalOid = new Oid();

            foreach (IOIDSettingDTO OIDSetting in OIDSettingCollection)
            {
                SNMPRunAgent(UDPtarget, pdu, param, OIDSetting, SNMPDeviceData);
            }
        }

        private void SNMPRunAgent(UdpTarget UDPtarget, Pdu pdu, AgentParameters param, IOIDSettingDTO OIDSetting, ISNMPDeviceDTO SNMPDeviceData)
        {
            bool nextEntry = true;
            SnmpV2Packet Result;
            Oid indexOid = new Oid(OIDSetting.InitialOID); //Walk control
            Oid finalOid = new Oid(OIDSetting.FinalOID);
            IEnumerable<Oid> IndexedOIDvalues = OIDSetting.IndexedOIDSettings.Keys.Select(x => new Oid(x)); //Collection of OIDs which have index format information

            //Test
            Console.WriteLine("----- {0} -----", OIDSetting.ID);

            while (nextEntry)
            {
                pdu.VbList.Add(indexOid); //Add starting OID for request

                try
                {
                    Result = (SnmpV2Packet)UDPtarget.Request(pdu, param);
                    nextEntry = SNMPDecodeData(Result, indexOid, finalOid, OIDSetting.InclusiveInterval, IndexedOIDvalues, SNMPDeviceData);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception {0} -- Hopping to next OIDSetting", e.Message);
                    nextEntry = false;
                }

                //Prepare PDU object for iteration. Otherwise, wipe contents of pdu
                pdu.RequestId++;
                pdu.VbList.Clear();
            }
        }

        private bool SNMPDecodeData(SnmpV2Packet Result, Oid indexOid, Oid finalOid, bool InclusiveInterval, IEnumerable<Oid> IndexedOIDvalues, ISNMPDeviceDTO SNMPDeviceData)
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
                        string rootOID = IndexedOIDvalues.FirstOrDefault(x => x.IsRootOf(ResBinding.Oid)) == null ? string.Empty : IndexedOIDvalues.FirstOrDefault(x => x.IsRootOf(ResBinding.Oid)).ToString();
                        ISNMPRawEntryDTO SNMPRawData = SNMPDeviceData.BuildSNMPRawEntry(ResBinding.Oid.ToString(), rootOID, ResBinding.Value.ToString(), (EnumSNMPOIDType)Enum.Parse(typeof(EnumSNMPOIDType), SnmpConstants.GetTypeName(ResBinding.Value.Type)));

                        //Test
                        Console.WriteLine("Request {0} || {1} -- {2} ({3}): {4}", SNMPDeviceData.TargetIP.ToString(), Result.Pdu.RequestId, SNMPRawData.OID, SNMPRawData.DataType.ToString(), SNMPRawData.ValueData);

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

        #region Mocks

        public void MockGetSettings()
        {
            ISNMPSettingDTO MockSNMPSetting = _model.BuildSNMPSetting("ColecciónSwitches", "192.168.1.42", "192.168.1.42", "public");
            ISNMPProcessingProfileDTO MockProcessProfileSetting = MockSNMPSetting.BuildProcessingProfile("TopologiaSwitches", EnumProcessingType.TopologyDiscovery);

            #region Step1
            
            //Test combination. When processing gets impelmented, it will be included on the algorithm
            MockProcessProfileSetting.BuildOIDSetting("Step1A - Basic Info", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8");
            IOIDSettingDTO MockOIDSettingB = MockProcessProfileSetting.BuildOIDSetting("Step2E - Port descriptive names", "1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.2");
            IList<EnumSNMPOIDIndexType> indexesB = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
            MockOIDSettingB.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.2", indexesB);

            MockProcessProfileSetting.BuildOIDSetting("Step1B - Port MAC Address", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6");
            MockProcessProfileSetting.BuildOIDSetting("Step2J - Learned MAC Address By Port ID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2");
            MockProcessProfileSetting.BuildOIDSetting("Step2I - VLAN detection by port (except Trunks)", "1.3.6.1.2.1.17.7.1.4.3.1", "1.3.6.1.2.1.17.7.1.4.3.1");

            //Extra pero no imprescindible
            IOIDSettingDTO MockOIDSettingC = MockProcessProfileSetting.BuildOIDSetting("Step1C - Learned MACs By Port MAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4");
            IList<EnumSNMPOIDIndexType> indexesC = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
            MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexesC);
            MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexesC);
            MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexesC);

            MockProcessProfileSetting.BuildOIDSetting("StepX - TEST", "1.2.840.10006.300.43", "1.2.840.10006.300.43");
            
            #endregion

            #region Step2

            //MockProcessProfileSetting.BuildOIDSetting("Step2A", "1.0.8802.1.1.2.1.4.2.1.4", "1.0.8802.1.1.2.1.4.2.1.4");
            //MockProcessProfileSetting.BuildOIDSetting("Step2B", "1.0.8802.1.1.2.1.4.1.1.7", "1.0.8802.1.1.2.1.4.1.1.7");
            //MockProcessProfileSetting.BuildOIDSetting("Step2C", "1.2.840.10006.300.43.1.1.1.1.7", "1.2.840.10006.300.43.1.1.1.1.7");
            //MockProcessProfileSetting.BuildOIDSetting("Step2D", "1.2.840.10006.300.43.1.2.1.1.5", "1.2.840.10006.300.43.1.2.1.1.5");
            //MockProcessProfileSetting.BuildOIDSetting("Step2F", "1.3.6.1.4.1.9.9.46.1.3.1.1.4", "1.3.6.1.4.1.9.9.46.1.3.1.1.4");
            //MockProcessProfileSetting.BuildOIDSetting("Step2G", "1.3.6.1.2.1.17.1.4.1.2", "1.3.6.1.2.1.17.1.4.1.2");
            //MockProcessProfileSetting.BuildOIDSetting("Step2H", "1.3.6.1.2.1.17.2.15.1.3", "1.3.6.1.2.1.17.2.15.1.3");
            //MockProcessProfileSetting.BuildOIDSetting("Step2K", "1.3.6.1.2.1.31.1.1.1.1", "1.3.6.1.2.1.31.1.1.1.1");
            //MockProcessProfileSetting.BuildOIDSetting("Step2L", "1.3.6.1.2.1.31.1.1.1.6", "1.3.6.1.2.1.31.1.1.1.6");
            //MockProcessProfileSetting.BuildOIDSetting("Step2M", "1.3.6.1.2.1.31.1.1.1.10", "1.3.6.1.2.1.31.1.1.1.10");
            //MockProcessProfileSetting.BuildOIDSetting("Step2N", "1.3.6.1.2.1.31.1.1.1.15", "1.3.6.1.2.1.31.1.1.1.15");
            //MockProcessProfileSetting.BuildOIDSetting("Step2Ñ", "1.3.6.1.2.1.31.1.1.1.18", "1.3.6.1.2.1.31.1.1.1.18");

            //1.3.6.1.4.1.11.2 --> nm Evitamos porque es propietario HP...
            #endregion

        }

        #endregion
    }
}
