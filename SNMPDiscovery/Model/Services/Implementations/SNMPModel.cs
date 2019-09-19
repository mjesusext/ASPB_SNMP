using System;
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

namespace SNMPDiscovery.Model.Services
{
    public class SNMPModel : ISNMPModelDTO
    {
        private const int DefaultPort = 161;
        private const int DefaultTimeout = 1000;
        private const int DefaultRetries = 1;
        private IList<IObserver<ISNMPModelDTO>> _snmpModelObservers;

        public IDictionary<string, ISNMPSettingDTO> SNMPSettings { get; set; }
        public IDictionary<string, ISNMPDeviceDTO> SNMPData { get; set; }
        public IDictionary<Type, IList> ChangedObjects { get; set; }

        #region Commands

        public void StartDiscovery()
        {
            //Iterate on all settings
            foreach (ISNMPSettingDTO SNMPSettingEntry in SNMPSettings.Values)
            {
                SNMPWalkByIP(SNMPSettingEntry);
            }
        }

        public void RunProcesses()
        {
            IEnumerable<ISNMPProcessStrategy> processes = SNMPSettings.SelectMany(x => x.Value.Processes.Values);

            foreach (ISNMPProcessStrategy alg in processes)
            {
                //Notify initialization
                Console.WriteLine($"Starting algorithm {alg.ProcessID}");

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

        #region Interface Implementations

        public IDisposable Subscribe(IObserver<ISNMPModelDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_snmpModelObservers.Contains(observer))
            {
                _snmpModelObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<ISNMPModelDTO>(_snmpModelObservers, observer);
        }

        public ISNMPSettingDTO BuildSNMPSetting(string ID, string initialIP, string finalIP, string SNMPUser)
        {
            //Lazy initialization
            if (SNMPSettings == null)
            {
                SNMPSettings = new Dictionary<string, ISNMPSettingDTO>();
            }

            ISNMPSettingDTO setting = new SNMPSettingDTO(ID, initialIP, finalIP, SNMPUser, ChangeTrackerHandler);
            SNMPSettings.Add(ID, setting);

            return setting;
        }

        public ISNMPDeviceDTO BuildSNMPDevice(string targetIP)
        {
            //Lazy initialization
            if (SNMPData == null)
            {
                SNMPData = new Dictionary<string, ISNMPDeviceDTO>();
            }

            ISNMPDeviceDTO device = new SNMPDeviceDTO(targetIP, ChangeTrackerHandler);
            SNMPData.Add(targetIP, device);

            return device;
        }

        public ISNMPDeviceDTO BuildSNMPDevice(int targetIP)
        {
            //Lazy initialization
            if (SNMPData == null)
            {
                SNMPData = new Dictionary<string, ISNMPDeviceDTO>();
            }

            ISNMPDeviceDTO device = new SNMPDeviceDTO(targetIP, ChangeTrackerHandler);
            SNMPData.Add(new IPAddress(targetIP).ToString(), device);

            return device;
        }

        #endregion

        #region Nested Object Change Handlers

        public void ChangeTrackerHandler(object obj, Type type)
        {
            if(ChangedObjects.Count == 0)
            {
                ChangedObjects.Add(type, new ArrayList() { obj });
            }
            else if (ChangedObjects.ContainsKey(type))
            {
                ChangedObjects[type].Add(obj);
            }
            else
            {
                foreach (IObserver<ISNMPModelDTO> observer in _snmpModelObservers)
                {
                    observer.OnNext(this);
                }

                ChangedObjects.Clear();
                ChangedObjects.Add(type, new ArrayList() { obj });
            }
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
                    ISNMPDeviceDTO device = BuildSNMPDevice(IPAddress.NetworkToHostOrder(i));
                    UDPtarget.Address = device.TargetIP;
                    SNMPWalkByOIDSetting(UDPtarget, pdu, param, SNMPSettingEntry, device);
                }
            }

            //Test
            Console.WriteLine("Cantidad de entradas: {0}", SNMPData.Values.FirstOrDefault().SNMPRawDataEntries.Count);
        }

        private void SNMPWalkByOIDSetting(UdpTarget UDPtarget, Pdu pdu, AgentParameters param, ISNMPSettingDTO SNMPSettingEntry, ISNMPDeviceDTO SNMPDeviceData)
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

        private void SNMPRunAgent(UdpTarget UDPtarget, Pdu pdu, AgentParameters param, IOIDSettingDTO OIDSetting, ISNMPDeviceDTO SNMPDeviceData)
        {
            bool nextEntry = true;
            SnmpV2Packet Result;
            Oid indexOid = new Oid(OIDSetting.InitialOID); //Walk control
            Oid finalOid = new Oid(OIDSetting.FinalOID);

            //Test
            Console.WriteLine("----- {0} -----", OIDSetting.ID);

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
                    Console.WriteLine("Exception {0} -- Hopping to next OIDSetting", e.Message);
                    nextEntry = false;
                }

                //Prepare PDU object for iteration. Otherwise, wipe contents of pdu
                pdu.RequestId++;
                pdu.VbList.Clear();
            }
        }

        private bool SNMPDecodeData(SnmpV2Packet Result, Oid indexOid, Oid finalOid, bool InclusiveInterval, ISNMPDeviceDTO SNMPDeviceData)
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

        #region Constructors

        public SNMPModel()
        {
            _snmpModelObservers = new List<IObserver<ISNMPModelDTO>>();
            ChangedObjects = new Dictionary<Type, IList>();

            ////Mock for redirecting console to file
            //FileStream ostrm;
            //StreamWriter writer;
            //TextWriter oldOut = Console.Out;
            //try
            //{
            //    ostrm = new FileStream("./Redirect.txt", FileMode.Create, FileAccess.Write);
            //    writer = new StreamWriter(ostrm);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Cannot open Redirect.txt for writing");
            //    Console.WriteLine(e.Message);
            //    return;
            //}
            //Console.SetOut(writer);

            //Mock methods for development
            MockGetSettings();
            StartDiscovery();
            RunProcesses();

            ////Mock for undoing things
            //Console.SetOut(oldOut);
            //writer.Close();
            //ostrm.Close();
        }

        #endregion

        #region Mocks

        public void MockGetSettings()
        {
            //ISNMPSettingDTO MockSNMPSetting = _model.BuildSNMPSetting("ColecciónSwitches", "192.168.1.42", "192.168.1.42", "public");
            ISNMPSettingDTO MockSNMPSetting = BuildSNMPSetting("ColecciónSwitches", "192.168.1.42", "192.168.1.51", "public");
            ISNMPProcessStrategy MockProcessProfileSetting = MockSNMPSetting.BuildProcess(EnumProcessingType.TopologyDiscovery);
        }

        #endregion
    }
}
