﻿using System;
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
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("DeviceBasicInfo", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8", false);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.None };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.1", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.2", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.3", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.4", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.5", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.6", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.1.7", indexes);

                OIDSettings.Add("DeviceBasicInfo", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("PhysPortDescription"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("PhysPortDescription", "1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.2", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.2", indexes);

                OIDSettings.Add("PhysPortDescription", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("PhysPortMACAddress"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("PhysPortMACAddress", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.6", indexes);

                OIDSettings.Add("PhysPortMACAddress", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("VLANDescription"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("VLANDescription", "1.3.6.1.2.1.17.7.1.4.3.1.1", "1.3.6.1.2.1.17.7.1.4.3.1.1", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.4.3.1.1", indexes);

                OIDSettings.Add("VLANDescription", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("VLANMapping"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("VLANDescription", "1.3.6.1.2.1.17.7.1.4.3.1.2", "1.3.6.1.2.1.17.7.1.4.3.1.2", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.4.3.1.2", indexes);

                OIDSettings.Add("VLANMapping", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortID"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("LearnedMACByPhysPortID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.MacAddress };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.2.2.1.2", indexes);

                OIDSettings.Add("LearnedMACByPhysPortID", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortMAC"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("LearnedMACByPhysPortMAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4", false);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexes);

                OIDSettings.Add("LearnedMACByPhysPortMAC", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("LACPSetting"))
            {
                //OIDSettings.Add("LACPSetting", new OIDSettingDTO("LACPSetting", "1.2.840.10006.300.43", "1.2.840.10006.300.43", true));
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("LACPSetting", "1.2.840.10006.300.43.1.1.2.1.1", "1.2.840.10006.300.43.1.1.2.1.1", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.2.840.10006.300.43.1.1.2.1.1", indexes);

                OIDSettings.Add("LACPSetting", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("PortHierarchy"))
            {
                IOIDSettingDTO MockOIDSetting = new OIDSettingDTO("PortHierarchy", "1.3.6.1.2.1.31.1.2.1.3", "1.3.6.1.2.1.31.1.2.1.3", true);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.31.1.2.1.3", indexes);

                OIDSettings.Add("PortHierarchy", MockOIDSetting);
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
                GetVLANInfo(Device, OIDSettings, TopologyInfo); //Get VLANInventory and mappings
                ComputeDirectNeighbours(TopologyInfo);

                int i = 4;
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
            MappingHandlers.Add(null);

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
            TopologyInfo.PortLearnedAddresses = new Dictionary<string, IDictionary<string, string>>();

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
            MappingHandlers.Add((x,y,z) => { ((ITopologyInfoDTO)z).PortMACAddress.Add(x[0], y); });

            //Define container if necesary
            TopologyInfo.PortMACAddress = new Dictionary<string, string>();

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
            MappingHandlers.Add((x, y, z) => { ((ITopologyInfoDTO)z).PortInventory.Add(x[0], y); });

            TopologyInfo.PortInventory = new Dictionary<string, string>();
            ModelHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);

            #endregion

            #region Port typology

            //By default, all ports are access ports. Initialize dictionary
            TopologyInfo.PortSettings = new Dictionary<string, CustomPair<EnumPhysPortType, string>>();
            
            foreach (string portID in TopologyInfo.PortMACAddress.Keys)
            {
                TopologyInfo.PortSettings.Add(portID, new CustomPair<EnumPhysPortType, string>(EnumPhysPortType.Access, null));
            } 

            //Detect ports without MAC --> type loopback
            IEnumerable<string> LoopbackPorts = TopologyInfo.PortMACAddress.Where(x => string.IsNullOrEmpty(x.Value)).Select(x => x.Key);
            foreach (string loopbackitem in LoopbackPorts)
            {
                TopologyInfo.PortSettings[loopbackitem].First = EnumPhysPortType.Loopback;
            }

            // Adrress by port > LearnedMACThreshold --> InferedTrunks
            IEnumerable<string> InferedTrunks = TopologyInfo.PortLearnedAddresses.Where(x => x.Value.Count > LearnedMACThreshold).Select(x => x.Key);
            foreach (string inferedtrunkitem in InferedTrunks)
            {
                TopologyInfo.PortSettings[inferedtrunkitem].First = EnumPhysPortType.InferedTrunk;
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
                TopologyInfo.PortSettings[vlanport].First = EnumPhysPortType.VirtualPort;
            }

            //Trunk ports (any protocol)
            IEnumerable<KeyValuePair<string, IList<string>>> trunkports = PortHierarchyResults.Where(x => x.Value.Count > 1 && x.Key != "0").Where(x => !PortHierarchyResults["0"].Contains(x.Key));

            foreach (KeyValuePair<string, IList<string>> trunkentry in trunkports)
            {
                TopologyInfo.PortSettings[trunkentry.Key].First = EnumPhysPortType.Trunk;

                foreach (string aggregateentry in trunkentry.Value)
                {
                    TopologyInfo.PortSettings[aggregateentry].First = EnumPhysPortType.Aggregate;
                    TopologyInfo.PortSettings[aggregateentry].Second = trunkentry.Key;
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
                TopologyInfo.PortSettings[lacpentry.Key].First = EnumPhysPortType.LACP;

                foreach (string groupedport in lacpentry.Value)
                {
                    if (TopologyInfo.PortSettings.ContainsKey(groupedport))
                    {
                        TopologyInfo.PortSettings[groupedport].First = EnumPhysPortType.Aggregate;
                        TopologyInfo.PortSettings[groupedport].Second = lacpentry.Key;
                    }
                }
            }

            #endregion
        }

        private void GetVLANInfo(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //Key: Port ID, Value: Tuple of VLAN ID and VLAN name
            //IDictionary<string, CustomPair<string, string>> VLANByInterfaceID { get; set; }

            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;
            IDictionary<string, string> VLANMappingResult;

            SelectedSetting = OIDSettings["VLANDescription"];
            MappingHandlers = new List<Action<IList<string>, string, object>>();

            #region VLAN description

            MappingHandlers.Add((x, y, z) => { ((IDictionary<string,string>)z).Add(x[0], y); });
            TopologyInfo.VLANInventory = new Dictionary<string, string>();

            ModelHelper.OIDEntryProcessor(Device, TopologyInfo.VLANInventory, SelectedSetting, MappingHandlers);

            #endregion

            #region VLAN Mapping

            //By default, all ports are access ports. Initialize dictionary
            MappingHandlers.Clear();
            MappingHandlers.Add((x,y,z) => { ((IDictionary<string, string>)z).Add(x[0], y); });

            VLANMappingResult = new Dictionary<string, string>();
            SelectedSetting = OIDSettings["VLANMapping"];

            ModelHelper.OIDEntryProcessor(Device, VLANMappingResult, SelectedSetting, MappingHandlers);

            #endregion

            #region Data Parsing

            TopologyInfo.PortVLANMapping = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string,string> VLANMaskInfo in VLANMappingResult)
            {
                string bitmask = ModelHelper.GetStringBitMask(VLANMaskInfo.Value);

                for (int i = 1; i <= bitmask.Length; i++)
                {
                    if (TopologyInfo.PortInventory.ContainsKey(i.ToString()))
                    {
                        if (TopologyInfo.PortVLANMapping.ContainsKey(i.ToString()))
                        {
                            if(bitmask[i-1] == '1')
                            {
                                TopologyInfo.PortVLANMapping[i.ToString()].Add(VLANMaskInfo.Key);
                            }
                        }
                        else
                        {
                            if(bitmask[i-1] == '1')
                            {
                                TopologyInfo.PortVLANMapping.Add(i.ToString(), new List<string>() { VLANMaskInfo.Key });
                            }
                        }
                    }
                }
            }

            #endregion
        }

        private void ComputeDirectNeighbours(ITopologyInfoDTO TopologyInfo)
        {
            //Search access type
            //IDictionary<string, CustomPair<EnumPhysPortType, string>> PortSettings;
            //Get learned MAC and IP
            //IDictionary<string, IDictionary<string, string>> PortLearnedAddresses;
            //Insert into dictionary
            //IDictionary<string, CustomPair<string, string>> DeviceDirectNeighbours;
        }

        #endregion

        #region Entry Processor Handlers

        private void LearnedAddressMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            ITopologyInfoDTO TopologyInfo = StrategyDTOobject as ITopologyInfoDTO;

            IDictionary<string, IDictionary<string, string>> LearnedAddress = TopologyInfo.PortLearnedAddresses;

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

            string bitmask = ModelHelper.GetStringBitMask(Value);

            for (int i = 0; i < bitmask.Length; i++)
            {
                if (bitmask[i] == '1')
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