using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Helpers;

namespace SNMPDiscovery.Model.Services
{
    public class TopologyBuilderStrategy : ISNMPProcessStrategy
    {
        private const int LearnedMACThreshold = 1;
        private IDictionary<string, string> MACAddrMapper { get; set; }

        public string ProcessID { get; }
        public string RegardingDeviceSetting { get; set; }
        public event Action<object, Type> OnChange;

        #region Interfaces implementations

        public IDictionary<string, IOIDSettingDTO> BuildOIDSetting(string regardingSetting, IDictionary<string, IOIDSettingDTO> OIDSettings)
        {
            IOIDSettingDTO MockOIDSetting;
            RegardingDeviceSetting = regardingSetting;

            //Lazy initialization
            if (OIDSettings == null)
            {
                OIDSettings = new Dictionary<string, IOIDSettingDTO>();
            }

            #region New Version - Pending of reading from persistance

            if (!OIDSettings.ContainsKey("DeviceBasicInfo"))
            {
                MockOIDSetting = new OIDSettingDTO("DeviceBasicInfo", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8", false, null);
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
                MockOIDSetting = new OIDSettingDTO("PhysPortDescription", "1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.2", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.2", indexes);

                OIDSettings.Add("PhysPortDescription", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("PhysPortMACAddress"))
            {
                MockOIDSetting = new OIDSettingDTO("PhysPortMACAddress", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.6", indexes);

                OIDSettings.Add("PhysPortMACAddress", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("VLANDescription"))
            {
                MockOIDSetting = new OIDSettingDTO("VLANDescription", "1.3.6.1.2.1.17.7.1.4.3.1.1", "1.3.6.1.2.1.17.7.1.4.3.1.1", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.4.3.1.1", indexes);

                OIDSettings.Add("VLANDescription", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("VLANMapping"))
            {
                MockOIDSetting = new OIDSettingDTO("VLANDescription", "1.3.6.1.2.1.17.7.1.4.3.1.2", "1.3.6.1.2.1.17.7.1.4.3.1.2", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.4.3.1.2", indexes);

                OIDSettings.Add("VLANMapping", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortID"))
            {
                MockOIDSetting = new OIDSettingDTO("LearnedMACByPhysPortID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.MacAddress };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.2.2.1.2", indexes);

                OIDSettings.Add("LearnedMACByPhysPortID", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortMAC"))
            {
                MockOIDSetting = new OIDSettingDTO("LearnedMACByPhysPortMAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4", false, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexes);

                OIDSettings.Add("LearnedMACByPhysPortMAC", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("LACPSetting"))
            {
                MockOIDSetting = new OIDSettingDTO("LACPSetting", "1.2.840.10006.300.43.1.1.2.1.1", "1.2.840.10006.300.43.1.1.2.1.1", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.2.840.10006.300.43.1.1.2.1.1", indexes);

                OIDSettings.Add("LACPSetting", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("PortHierarchy"))
            {
                MockOIDSetting = new OIDSettingDTO("PortHierarchy", "1.3.6.1.2.1.31.1.2.1.3", "1.3.6.1.2.1.31.1.2.1.3", true, null);
                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.Number };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.2.1.31.1.2.1.3", indexes);

                OIDSettings.Add("PortHierarchy", MockOIDSetting);
            }

            if (!OIDSettings.ContainsKey("TrunkDestinationsCDP"))
            {
                MockOIDSetting = new OIDSettingDTO("TrunkDestinationsCDP", "1.3.6.1.4.1.9.9.23.1.2.1.1.6", "1.3.6.1.4.1.9.9.23.1.2.1.1.7", true, null);

                IList<EnumSNMPOIDIndexType> indexes = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.None };
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.4.1.9.9.23.1.2.1.1.6", indexes);
                MockOIDSetting.BuildIndexedOIDSetting("1.3.6.1.4.1.9.9.23.1.2.1.1.7", indexes);

                OIDSettings.Add("TrunkDestinationsCDP", MockOIDSetting);
            }

            #region MJE - TEST

            //MockOIDSetting = new OIDSettingDTO("Step2A", "1.0.8802.1.1.2.1.4.2.1.4", "1.0.8802.1.1.2.1.4.2.1.4", true, null);
            //OIDSettings.Add("Step2A", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2B", "1.0.8802.1.1.2.1.4.1.1.7", "1.0.8802.1.1.2.1.4.1.1.7", false, null);
            //OIDSettings.Add("Step2B", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2C", "1.2.840.10006.300.43.1.1.1.1.7", "1.2.840.10006.300.43.1.1.1.1.7", true, null);
            //OIDSettings.Add("Step2C", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2D", "1.2.840.10006.300.43.1.2.1.1.5", "1.2.840.10006.300.43.1.2.1.1.5", true, null);
            //OIDSettings.Add("Step2D", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2F", "1.3.6.1.4.1.9.9.46.1.3.1.1.4", "1.3.6.1.4.1.9.9.46.1.3.1.1.4", true, null);
            //OIDSettings.Add("Step2F", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2G", "1.3.6.1.2.1.17.1.4.1.2", "1.3.6.1.2.1.17.1.4.1.2", true, null);
            //OIDSettings.Add("Step2G", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2H", "1.3.6.1.2.1.17.2.15.1.3", "1.3.6.1.2.1.17.2.15.1.3", true, null);
            //OIDSettings.Add("Step2H", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2K", "1.3.6.1.2.1.31.1.1.1.1", "1.3.6.1.2.1.31.1.1.1.1", true, null);
            //OIDSettings.Add("Step2K", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2L", "1.3.6.1.2.1.31.1.1.1.6", "1.3.6.1.2.1.31.1.1.1.6", true, null);
            //OIDSettings.Add("Step2L", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2M", "1.3.6.1.2.1.31.1.1.1.10", "1.3.6.1.2.1.31.1.1.1.10", true, null);
            //OIDSettings.Add("Step2M", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2N", "1.3.6.1.2.1.31.1.1.1.15", "1.3.6.1.2.1.31.1.1.1.15", true, null);
            //OIDSettings.Add("Step2N", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("Step2Ñ", "1.3.6.1.2.1.31.1.1.1.18", "1.3.6.1.2.1.31.1.1.1.18", true, null);
            //OIDSettings.Add("Step2Ñ", MockOIDSetting);
            //
            //MockOIDSetting = new OIDSettingDTO("PortHierarchyTEST", "1.3.6.1.2.1.31.1.2", "1.3.6.1.2.1.31.1.2", true, null);
            //OIDSettings.Add("PortHierarchyTEST", MockOIDSetting);

            #endregion

            #endregion

            //Data is fully ready for displaying
            //foreach (IOIDSettingDTO OIDdefs in OIDSettings.Values)
            //{
            //    OnChange?.Invoke(OIDdefs, typeof(IOIDSettingDTO));
            //}

            return OIDSettings;
        }

        public void Run(ISNMPModelDTO Model)
        {
            GetMACAddressMappings(Model);
            TransformRawData(Model);
            ComputeDirectNeighbours(Model);
            BuildTopology(Model);

            //We know data is fully ready
            foreach (var procres in Model.SNMPDeviceData.Values.SelectMany(x => x.SNMPProcessedData.Values))
            { 
                OnChange?.Invoke(procres, typeof(ISNMPProcessedValueDTO));
            }
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

        private void GetMACAddressMappings(ISNMPModelDTO Model)
        {
            ISNMPDeviceSettingDTO DeviceSett;
            IList<IPAddress> IPinventory;

            DeviceSett = Model.SNMPDeviceSettings[RegardingDeviceSetting];
            IPinventory = ModelHelper.GenerateFullHostList(DeviceSett.InitialIP, DeviceSett.NetworkMask);

            MACAddrMapper = new Dictionary<string, string>();

            foreach (string iptarget in IPinventory.Select(x=> x.ToString()))
            {
                MACAddrMapper.Add(ModelHelper.GetMACAddress(iptarget), iptarget);
            }
        }

        private void TransformRawData(ISNMPModelDTO Model)
        {
            IDictionary<string, IOIDSettingDTO> OIDSettings = Model.SNMPDeviceSettings[RegardingDeviceSetting].OIDSettings;

            foreach (ISNMPDeviceDataDTO Device in Model.SNMPDeviceData.Values)
            {
                //Create DTO and attach to device
                IDeviceTopologyInfoDTO TopologyInfo = new TopologyInfoDTO();
                ISNMPProcessedValueDTO DataContainer = Device.AttachSNMPProcessedValue(typeof(IDeviceTopologyInfoDTO), TopologyInfo);

                GetBasicInfo(Device, OIDSettings, TopologyInfo); //Fill with basic info
                GetLearnedMACAddresses(Device, OIDSettings, TopologyInfo); //Fill with LearnedAddress inventory
                GetPortMACAddress(Device, OIDSettings, TopologyInfo); //Fill with MAC address of each port
                GetPortIDInfo(Device, OIDSettings, TopologyInfo); //Fill with port IDs inventory
                GetVLANInfo(Device, OIDSettings, TopologyInfo); //Get VLANInventory and mappings
                GetAggregateDestinations(Device, OIDSettings, TopologyInfo); //Get destinations of each aggregate / infered trunk 
            }
        }

        private void ComputeDirectNeighbours(ISNMPModelDTO Model)
        {
            foreach (ISNMPDeviceDataDTO Device in Model.SNMPDeviceData.Values)
            {
                IDeviceTopologyInfoDTO DeviceTopology = (IDeviceTopologyInfoDTO)Device.SNMPProcessedData[nameof(IDeviceTopologyInfoDTO)].Data;
                DeviceTopology.DeviceDirectNeighbours = new Dictionary<string, IDictionary<string, string>>();

                //1) Get access ports
                //1.2) Get Learned MAC from access port
                IEnumerable<string> AccessPorts = DeviceTopology.PortSettings.Where(x => x.Value.First == EnumPhysPortType.Access).Select(x => x.Key);
                
                foreach (string acports in AccessPorts)
                {
                    if (DeviceTopology.PortLearnedAddresses.ContainsKey(acports))
                    {
                        DeviceTopology.DeviceDirectNeighbours.Add(acports, DeviceTopology.PortLearnedAddresses[acports]);
                    }
                }

                //2) Get Aggregates - InferedTrunks
                //2.2) Get CISCO OID for SWITCH MAC and port
                //2.3) Get Interface MAC linked to port of SWITCH MAC

                foreach (KeyValuePair<string, CustomPair<string,string>> aggports in DeviceTopology.PortAggregateDestinations)
                {
                    string deviceip; 

                    if (MACAddrMapper.TryGetValue(aggports.Value.First, out deviceip))
                    {
                        string PortMACAddr;
                        IDeviceTopologyInfoDTO targdevice = (IDeviceTopologyInfoDTO) Model.SNMPDeviceData[deviceip].SNMPProcessedData[nameof(IDeviceTopologyInfoDTO)].Data;

                        Dictionary<string, string> aggres = new Dictionary<string, string>();

                        //Try search by index first, otherwise by port description
                        if(targdevice.PortMACAddress.TryGetValue(aggports.Value.Second, out PortMACAddr))
                        {
                            aggres.Add(PortMACAddr, MACAddrMapper[aggports.Value.First]);
                        }
                        else if (targdevice.PortInventory.TryGetValue(aggports.Value.Second, out PortMACAddr))
                        {
                            aggres.Add(PortMACAddr, MACAddrMapper[aggports.Value.First]);
                        }

                        DeviceTopology.DeviceDirectNeighbours.Add(aggports.Key, aggres);
                    }
                }
            }
        }

        private void BuildTopology(ISNMPModelDTO Model)
        {
        }

        private void GetBasicInfo(ISNMPDeviceDataDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, IDeviceTopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;
            
            //Get setting of interest
            SelectedSetting = OIDSettings["DeviceBasicInfo"];

            //Define handle collection in order
            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).Description = y; });
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).OIDobjectID = y; });
            MappingHandlers.Add(null);
            MappingHandlers.Add(null);
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).DeviceName = y; });
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).Location = y; });
            MappingHandlers.Add(null);

            //Collect data mapping with handlers
            StrategyHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        private void GetLearnedMACAddresses(ISNMPDeviceDataDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, IDeviceTopologyInfoDTO TopologyInfo)
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
            StrategyHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        private void GetPortMACAddress(ISNMPDeviceDataDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, IDeviceTopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;

            //Get setting of interest 
            SelectedSetting = OIDSettings["PhysPortMACAddress"];

            //Define handle collection in order
            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x,y,z) => { ((IDeviceTopologyInfoDTO)z).PortMACAddress.Add(x[0], y); });

            //Define container if necesary
            TopologyInfo.PortMACAddress = new Dictionary<string, string>();

            //Collect data mapping with handlers
            StrategyHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        private void GetPortIDInfo(ISNMPDeviceDataDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, IDeviceTopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;
            IDictionary<string, IList<string>> LACPResults;
            IDictionary<string, IList<string>> PortHierarchyResults;

            #region Port description

            SelectedSetting = OIDSettings["PhysPortDescription"];

            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).PortInventory.Add(x[0], y); });

            TopologyInfo.PortInventory = new Dictionary<string, string>();
            StrategyHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);

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

            StrategyHelper.OIDEntryProcessor(Device, PortHierarchyResults, SelectedSetting, MappingHandlers);

            if (PortHierarchyResults.Count != 0)
            {
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
            }

            //LACP - Aggregate
            MappingHandlers.Clear();
            MappingHandlers.Add(LACPAssignmentMapper);
            LACPResults = new Dictionary<string, IList<string>>();
            SelectedSetting = OIDSettings["LACPSetting"];

            StrategyHelper.OIDEntryProcessor(Device, LACPResults, SelectedSetting, MappingHandlers);

            if(LACPResults.Count != 0)
            {
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
            }

            #endregion
        }

        private void GetVLANInfo(ISNMPDeviceDataDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, IDeviceTopologyInfoDTO TopologyInfo)
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

            StrategyHelper.OIDEntryProcessor(Device, TopologyInfo.VLANInventory, SelectedSetting, MappingHandlers);

            #endregion

            #region VLAN Mapping

            //By default, all ports are access ports. Initialize dictionary
            MappingHandlers.Clear();
            MappingHandlers.Add((x,y,z) => { ((IDictionary<string, string>)z).Add(x[0], y); });

            VLANMappingResult = new Dictionary<string, string>();
            SelectedSetting = OIDSettings["VLANMapping"];

            StrategyHelper.OIDEntryProcessor(Device, VLANMappingResult, SelectedSetting, MappingHandlers);

            TopologyInfo.PortVLANMapping = new Dictionary<string, List<string>>();

            if(VLANMappingResult.Count != 0)
            {
                foreach (KeyValuePair<string, string> VLANMaskInfo in VLANMappingResult)
                {
                    string[] positions = StrategyHelper.GetFlagArrayPositions(VLANMaskInfo.Value);

                    for (int i = 0; i < positions.Length; i++)
                    {
                        if (TopologyInfo.PortInventory.ContainsKey(positions[i]))
                        {
                            if (TopologyInfo.PortVLANMapping.ContainsKey(positions[i]))
                            {
                                TopologyInfo.PortVLANMapping[positions[i]].Add(VLANMaskInfo.Key);
                            }
                            else
                            {
                                TopologyInfo.PortVLANMapping.Add(positions[i], new List<string>() { VLANMaskInfo.Key });
                            }
                        }
                    }
                }
            }

            #endregion

        }

        private void GetAggregateDestinations(ISNMPDeviceDataDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, IDeviceTopologyInfoDTO TopologyInfo)
        {
            IOIDSettingDTO SelectedSetting;
            IList<Action<IList<string>, string, object>> MappingHandlers;

            //Get setting of interest 
            SelectedSetting = OIDSettings["TrunkDestinationsCDP"];

            //Define handle collection in order
            MappingHandlers = new List<Action<IList<string>, string, object>>();
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).PortAggregateDestinations.Add(x[0], new CustomPair<string, string>(y, null)); });
            MappingHandlers.Add((x, y, z) => { ((IDeviceTopologyInfoDTO)z).PortAggregateDestinations[x[0]].Second = y; });

            //Define container if necesary
            TopologyInfo.PortAggregateDestinations = new Dictionary<string, CustomPair<string,string>>();

            //Collect data mapping with handlers
            StrategyHelper.OIDEntryProcessor(Device, TopologyInfo, SelectedSetting, MappingHandlers);
        }

        #endregion

        #region Entry Processor Handlers

        private void LearnedAddressMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            IDeviceTopologyInfoDTO TopologyInfo = StrategyDTOobject as IDeviceTopologyInfoDTO;

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

            string[] positions = StrategyHelper.GetFlagArrayPositions(Value);

            for (int i = 0; i < positions.Length; i++)
            {
                LACPMapping[IndexValues[0]].Add(positions[i]);
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

        #region Constructor

        public TopologyBuilderStrategy(Action<object, Type> ChangeTrackerHandler)
        {
            ProcessID = "TopologyBuilder";
            OnChange += ChangeTrackerHandler;
        }

        #endregion
    }
}