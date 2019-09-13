using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Helpers;

namespace SNMPDiscovery.Model.Services
{
    public class TopologyBuilderStrategy : ISNMPProcessStrategy
    {
        public const int LearnedMACThreshold = 3;

        public string ProcessID { get; } = "TopologyBuilder";
        public string RegardingSetting { get; set; }

        #region Interfaces implementations

        public IDictionary<string, IOIDSettingDTO> BuildOIDSetting(string regardingSetting, IDictionary<string, IOIDSettingDTO> OIDSettings)
        {
            RegardingSetting = regardingSetting;

            //Lazy initialization
            if (OIDSettings == null)
            {
                OIDSettings = new Dictionary<string, IOIDSettingDTO>();
            }

            #region New Version - Pending of reading from persistance

            if (!OIDSettings.ContainsKey("DeviceBasicInfo"))
            {
                IOIDSettingDTO MockOIDSettingA = new OIDSettingDTO("DeviceBasicInfo", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8", false);
                IList<EnumSNMPOIDIndexType> indexesA = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.None };
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.1", indexesA);
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.2", indexesA);
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.3", indexesA);
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.4", indexesA);
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.5", indexesA);
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.6", indexesA);
                MockOIDSettingA.BuildIndexedOIDSetting("1.3.6.1.2.1.1.7", indexesA);

                OIDSettings.Add("DeviceBasicInfo", MockOIDSettingA);
            }

            if (!OIDSettings.ContainsKey("PhysPortDescription"))
            {
                IOIDSettingDTO MockOIDSettingB = new OIDSettingDTO("PhysPortDescription", "1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.2", true);
                IList<EnumSNMPOIDIndexType> indexesB = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSettingB.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.2", indexesB);

                OIDSettings.Add("PhysPortDescription", MockOIDSettingB);
            }

            if (!OIDSettings.ContainsKey("PhysPortMACAddress"))
            {
                IOIDSettingDTO MockOIDSettingC = new OIDSettingDTO("PhysPortMACAddress", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6", true);
                IList<EnumSNMPOIDIndexType> indexesC = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.6", indexesC);

                OIDSettings.Add("PhysPortMACAddress", MockOIDSettingC);
            }

            if (!OIDSettings.ContainsKey("VLANInfo"))
            {
                OIDSettings.Add("VLANInfo", new OIDSettingDTO("VLANInfo", "1.3.6.1.2.1.17.7.1.4.3.1", "1.3.6.1.2.1.17.7.1.4.3.1", true));
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortID"))
            {
                IOIDSettingDTO MockOIDSettingD = new OIDSettingDTO("LearnedMACByPhysPortID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2", true);
                IList<EnumSNMPOIDIndexType> indexesD = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.MacAddress };
                MockOIDSettingD.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.2.2.1.2", indexesD);

                OIDSettings.Add("LearnedMACByPhysPortID", MockOIDSettingD);
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortMAC"))
            {
                IOIDSettingDTO MockOIDSettingE = new OIDSettingDTO("LearnedMACByPhysPortMAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4", false);
                IList<EnumSNMPOIDIndexType> indexesE = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
                MockOIDSettingE.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexesE);
                MockOIDSettingE.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexesE);
                MockOIDSettingE.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexesE);

                OIDSettings.Add("LearnedMACByPhysPortMAC", MockOIDSettingE);
            }

            if (!OIDSettings.ContainsKey("LACPSetting"))
            {
                //OIDSettings.Add("LACPSetting", new OIDSettingDTO("LACPSetting", "1.2.840.10006.300.43", "1.2.840.10006.300.43", true));
                IOIDSettingDTO MockOIDSettingF = new OIDSettingDTO("LACPSetting", "1.2.840.10006.300.43.1.1.2.1.1", "1.2.840.10006.300.43.1.1.2.1.1", true);
                IList<EnumSNMPOIDIndexType> indexesF = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSettingF.BuildIndexedOIDSetting("1.2.840.10006.300.43.1.1.2.1.1", indexesF);

                OIDSettings.Add("LACPSetting", MockOIDSettingF);
            }

            if (!OIDSettings.ContainsKey("PortHierarchy"))
            {
                IOIDSettingDTO MockOIDSettingG = new OIDSettingDTO("PortHierarchy", "1.3.6.1.2.1.31.1.2.1.3", "1.3.6.1.2.1.31.1.2.1.3", true);
                IList<EnumSNMPOIDIndexType> indexesG = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.Number };
                MockOIDSettingG.BuildIndexedOIDSetting("1.3.6.1.2.1.31.1.2.1.3", indexesG);

                OIDSettings.Add("PortHierarchy", MockOIDSettingG);
            }


            //OIDSettings.Add("Step2A", new OIDSettingDTO("Step2A", "1.0.8802.1.1.2.1.4.2.1.4", "1.0.8802.1.1.2.1.4.2.1.4", true));
            //OIDSettings.Add("Step2B", new OIDSettingDTO("Step2B", "1.0.8802.1.1.2.1.4.1.1.7", "1.0.8802.1.1.2.1.4.1.1.7", false));
            //OIDSettings.Add("Step2C", new OIDSettingDTO("Step2C", "1.2.840.10006.300.43.1.1.1.1.7", "1.2.840.10006.300.43.1.1.1.1.7", true));
            //OIDSettings.Add("Step2D", new OIDSettingDTO("Step2D", "1.2.840.10006.300.43.1.2.1.1.5", "1.2.840.10006.300.43.1.2.1.1.5", true));
            //OIDSettings.Add("Step2F", new OIDSettingDTO("Step2F", "1.3.6.1.4.1.9.9.46.1.3.1.1.4", "1.3.6.1.4.1.9.9.46.1.3.1.1.4", true));
            //OIDSettings.Add("Step2G", new OIDSettingDTO("Step2G", "1.3.6.1.2.1.17.1.4.1.2", "1.3.6.1.2.1.17.1.4.1.2", true));
            //OIDSettings.Add("Step2H", new OIDSettingDTO("Step2H", "1.3.6.1.2.1.17.2.15.1.3", "1.3.6.1.2.1.17.2.15.1.3", true));
            //OIDSettings.Add("Step2K", new OIDSettingDTO("Step2K", "1.3.6.1.2.1.31.1.1.1.1", "1.3.6.1.2.1.31.1.1.1.1", true));
            //OIDSettings.Add("Step2L", new OIDSettingDTO("Step2L", "1.3.6.1.2.1.31.1.1.1.6", "1.3.6.1.2.1.31.1.1.1.6", true));
            //OIDSettings.Add("Step2M", new OIDSettingDTO("Step2M", "1.3.6.1.2.1.31.1.1.1.10", "1.3.6.1.2.1.31.1.1.1.10", true));
            //OIDSettings.Add("Step2N", new OIDSettingDTO("Step2N", "1.3.6.1.2.1.31.1.1.1.15", "1.3.6.1.2.1.31.1.1.1.15", true));
            //OIDSettings.Add("Step2Ñ", new OIDSettingDTO("Step2Ñ", "1.3.6.1.2.1.31.1.1.1.18", "1.3.6.1.2.1.31.1.1.1.18", true));
            //OIDSettings.Add("PortHierarchy", new OIDSettingDTO("PortHierarchy", "1.3.6.1.2.1.31.1.2", "1.3.6.1.2.1.31.1.2.1.3", true));

            #endregion

            return OIDSettings;
        }

        public void Run(ISNMPModelDTO Model)
        {
            TransformRawData(Model);
            BuildTopology(Model);
        }

        public void ValidateInput(ISNMPModelDTO Model)
        {
            //bool[] valResults = new bool[5];
            ////Check if the exists entries of specified OID ranges for every Device
            //
            //foreach (ISNMPDeviceDTO Device in Model.SNMPData.Values)
            //{
            //    //Example...
            //    valResults[0] = Device.SNMPRawDataEntries.Any(x => x.Key.StartsWith("1.3.6.1.2.1.1.1")); //Basic info
            //    
            //    if (valResults.All(x => x == true))
            //    {
            //        //Test
            //        Console.WriteLine($"Device with IP {Device.TargetIP.ToString()} contains requiered OIDs");
            //    }
            //    else
            //    {
            //        //Test
            //        Console.WriteLine($"Device with IP {Device.TargetIP.ToString()} does not contains requiered OIDs");
            //    }
            //}
        }

        #endregion

        #region Private Methods

        private void TransformRawData(ISNMPModelDTO Model)
        {
            IDictionary<string, IOIDSettingDTO> OIDSettings = Model.SNMPSettings[RegardingSetting].OIDSettings;

            foreach (ISNMPDeviceDTO Device in Model.SNMPData.Values)
            {
                //Create DTO and attach to device
                ITopologyInfoDTO TopologyInfo = new TopologyInfoDTO();
                Device.AttachSNMPProcessedValue(typeof(ITopologyInfoDTO), TopologyInfo);

                GetBasicInfo(Device, OIDSettings, TopologyInfo); //Fill with basic info
                GetLearnedMACAddresses(Device, OIDSettings, TopologyInfo); //Fill with LearnedAddress inventory
                GetPortMACAddress(Device, OIDSettings, TopologyInfo); //Fill with MAC address of each port
                GetPortIDInfo(Device, OIDSettings, TopologyInfo); //Fill with port IDs inventory

                int i = 3;
                //Fill with VLAN inventory
                //Fill with DirectNeighbours
            }
        }

        private void BuildTopology(ISNMPModelDTO Model)
        {

        }

        private void GetBasicInfo(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;
            
            //Get setting of interest
            SelectedSetting = OIDSettings["DeviceBasicInfo"];

            //Define handle collection in order
            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).Description = y; });
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).OIDobjectID = y; });
            MappingHandlers.Add(null);
            MappingHandlers.Add(null);
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).DeviceName = y; });
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).Location = y; });
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).Description = y; });

            //Collect data mapping with handlers
            ModelHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        private void GetLearnedMACAddresses(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;

            //Get setting of interest
            SelectedSetting = OIDSettings["LearnedMACByPhysPortID"];

            //Define handle collection in order
            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add(LearnedAddressMapper);

            //Initialize container if necesary
            TopologyInfo.LearnedAddressByInterfaceID = new Dictionary<string, IDictionary<string, string>>();

            //Collect data mapping with handlers
            ModelHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        private void GetPortMACAddress(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;

            //Get setting of interest 
            SelectedSetting = OIDSettings["PhysPortMACAddress"];

            //Define handle collection in order
            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x,y,z) => { ((ITopologyInfoDTO)z).MACPortByInterfaceID.Add(x[0], y); });

            //Define container if necesary
            TopologyInfo.MACPortByInterfaceID = new Dictionary<string, string>();

            //Collect data mapping with handlers
            ModelHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        private void GetPortIDInfo(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;
            IDictionary<string, IList<string>> LACPResults;
            IDictionary<string, IList<string>> PortHierarchyResults;

            #region Port description

            SelectedSetting = OIDSettings["PhysPortDescription"];

            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).PortsDescriptionByInterfaceID.Add(x[0], y); });

            TopologyInfo.PortsDescriptionByInterfaceID = new Dictionary<string, string>();
            ModelHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);

            #endregion

            #region Port typology

            //By default, all ports are access ports. Initialize dictionary
            TopologyInfo.PortsTypologyByInterfaceID = new Dictionary<string, CustomPair<EnumPhysPortType, string>>();
            
            foreach (string portID in TopologyInfo.MACPortByInterfaceID.Keys)
            {
                TopologyInfo.PortsTypologyByInterfaceID.Add(portID, new CustomPair<EnumPhysPortType, string>(EnumPhysPortType.Access, null));
            } 

            //Detect ports without MAC --> type loopback
            IEnumerable<string> LoopbackPorts = TopologyInfo.MACPortByInterfaceID.Where(x => string.IsNullOrEmpty(x.Value)).Select(x => x.Key);
            foreach (string loopbackitem in LoopbackPorts)
            {
                TopologyInfo.PortsTypologyByInterfaceID[loopbackitem].First = EnumPhysPortType.Loopback;
            }

            // Adrress by port > LearnedMACThreshold --> InferedTrunks
            IEnumerable<string> InferedTrunks = TopologyInfo.LearnedAddressByInterfaceID.Where(x => x.Value.Count > LearnedMACThreshold).Select(x => x.Key);
            foreach (string inferedtrunkitem in InferedTrunks)
            {
                TopologyInfo.PortsTypologyByInterfaceID[inferedtrunkitem].First = EnumPhysPortType.InferedTrunk;
            }

            // VirtualPort - Trunk
            MappingHandlers.Clear();
            MappingHandlers.Add(PortHierarchyMapper);
            PortHierarchyResults = new Dictionary<string, IList<string>>();
            SelectedSetting = OIDSettings["PortHierarchy"];

            ModelHelper.OIDEntryProcessor(Device, PortHierarchyResults, SelectedSetting, MappingHandlers);

            //Virtual ports
            foreach (string vlanport in PortHierarchyResults["0"])
            {
                TopologyInfo.PortsTypologyByInterfaceID[vlanport].First = EnumPhysPortType.VirtualPort;
            }

            //Trunk ports (any protocol)
            
            //MJE WRONG QUERY... Check it 
            IEnumerable<KeyValuePair<string, IList<string>>> trunkports = PortHierarchyResults.Where(x => x.Value.Count > 1 && !PortHierarchyResults["0"].Contains(x.Key));

            foreach (KeyValuePair<string, IList<string>> trunkentry in trunkports)
            {
                TopologyInfo.PortsTypologyByInterfaceID[trunkentry.Key].First = EnumPhysPortType.Trunk;

                foreach (string aggregateentry in trunkentry.Value)
                {
                    TopologyInfo.PortsTypologyByInterfaceID[aggregateentry].First = EnumPhysPortType.Aggregate;
                    TopologyInfo.PortsTypologyByInterfaceID[aggregateentry].Second = trunkentry.Key;
                }
            }

            //LACP - Aggregate
            MappingHandlers.Clear();
            MappingHandlers.Add(LACPAssignmentMapper);
            LACPResults = new Dictionary<string, IList<string>>();
            SelectedSetting = OIDSettings["LACPSetting"];

            ModelHelper.OIDEntryProcessor(Device, LACPResults, SelectedSetting, MappingHandlers);

            foreach (KeyValuePair<string, IList<string>> lacpentry in LACPResults)
            {
                TopologyInfo.PortsTypologyByInterfaceID[lacpentry.Key].First = EnumPhysPortType.LACP;

                foreach (string groupedport in lacpentry.Value)
                {
                    TopologyInfo.PortsTypologyByInterfaceID[groupedport].First = EnumPhysPortType.Aggregate;
                    TopologyInfo.PortsTypologyByInterfaceID[groupedport].Second = lacpentry.Key;
                }
            }

            #endregion
        }

        private void GetVLANInfo(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //Get info with handler that gets string of flags
            //Post processing of those flags
        }

        #endregion

        #region Entry Processor Handlers

        private void LearnedAddressMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            ITopologyInfoDTO TopologyInfo = StrategyDTOobject as ITopologyInfoDTO;

            IDictionary<string, IDictionary<string, string>> LearnedAddress = TopologyInfo.LearnedAddressByInterfaceID;

            if (LearnedAddress.ContainsKey(Value))
            {
                if (!LearnedAddress[Value].ContainsKey(IndexValues[1]))
                {
                    LearnedAddress[Value].Add(IndexValues[1], null);
                }
            }
            else
            {
                LearnedAddress.Add(Value, new Dictionary<string, string>() { { IndexValues[1], null } });
            }
        }

        private void LACPAssignmentMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            IDictionary<string, IList<string>> LACPMapping = StrategyDTOobject as IDictionary<string, IList<string>>;
            LACPMapping.Add(IndexValues[0], new List<string>());

            char[] flagarray = string.Join("",Value.Split(' ').Select(x => int.Parse(x)).Select(x => Convert.ToString(x, 2))).ToCharArray();

            for (int i = 0; i < flagarray.Length; i++)
            {
                if(flagarray[i] == '1')
                {
                    LACPMapping[IndexValues[0]].Add((i + 1).ToString());
                }
            }
        }

        private void PortHierarchyMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            IDictionary<string, IList<string>> HierarchyMapping;
            
            if(Value != "1")
            {
                return;
            }

            HierarchyMapping = StrategyDTOobject as IDictionary<string, IList<string>>;

            if (!HierarchyMapping.ContainsKey(IndexValues[0]))
            {
                HierarchyMapping.Add(IndexValues[0], new List<string>());
            }
            
            HierarchyMapping[IndexValues[0]].Add(IndexValues[1]);

        }

        #endregion
    }
}