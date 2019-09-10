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
        public string ProcessID { get; } = "TopologyBuilder";
        public string RegardingSetting { get; set; }

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
                OIDSettings.Add("DeviceBasicInfo", new OIDSettingDTO("Step1A - Basic Info", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8", false));
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
                OIDSettings.Add("PhysPortMACAddress", new OIDSettingDTO("PhysPortMACAddress", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6", true));
            }

            if (!OIDSettings.ContainsKey("VLANInfo"))
            {
                OIDSettings.Add("VLANInfo", new OIDSettingDTO("VLANInfo", "1.3.6.1.2.1.17.7.1.4.3.1", "1.3.6.1.2.1.17.7.1.4.3.1", true));
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortID"))
            {
                IOIDSettingDTO MockOIDSettingB = new OIDSettingDTO("LearnedMACByPhysPortID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2", true);
                IList<EnumSNMPOIDIndexType> indexesB = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number, EnumSNMPOIDIndexType.MacAddress };
                MockOIDSettingB.BuildIndexedOIDSetting("1.3.6.1.2.1.17.7.1.2.2.1.2", indexesB);

                OIDSettings.Add("LearnedMACByPhysPortID", MockOIDSettingB);
            }

            if (!OIDSettings.ContainsKey("LearnedMACByPhysPortMAC"))
            {
                IOIDSettingDTO MockOIDSettingC = new OIDSettingDTO("LearnedMACByPhysPortMAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4", false);
                IList<EnumSNMPOIDIndexType> indexesC = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexesC);
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexesC);
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexesC);

                OIDSettings.Add("LearnedMACByPhysPortMAC", MockOIDSettingC);
            }

            if (!OIDSettings.ContainsKey("LACPSetting"))
            {
                OIDSettings.Add("LACPSetting", new OIDSettingDTO("LACPSetting", "1.2.840.10006.300.43", "1.2.840.10006.300.43", true));
            }



            OIDSettings.Add("Step2A", new OIDSettingDTO("Step2A", "1.0.8802.1.1.2.1.4.2.1.4", "1.0.8802.1.1.2.1.4.2.1.4", true));
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
            OIDSettings.Add("Step2Ñ", new OIDSettingDTO("Step2Ñ", "1.3.6.1.2.1.31.1.1.1.18", "1.3.6.1.2.1.31.1.1.1.18", true));

            #endregion

            #region Old version

            //Test combination. When processing gets impelmented, it will be included on the algorithm
            //BuildOIDSetting("Step1A - Basic Info", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8");
            //IOIDSettingDTO MockOIDSettingB = BuildOIDSetting("Step2E - Port descriptive names", "1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.2");
            //IList<EnumSNMPOIDIndexType> indexesB = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
            //MockOIDSettingB.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.2", indexesB);
            //BuildOIDSetting("Step1B - Port MAC Address", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6");
            //BuildOIDSetting("Step2J - Learned MAC Address By Port ID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2");
            //BuildOIDSetting("Step2I - VLAN detection by port (except Trunks)", "1.3.6.1.2.1.17.7.1.4.3.1", "1.3.6.1.2.1.17.7.1.4.3.1");

            //Extra pero no imprescindible
            //IOIDSettingDTO MockOIDSettingC = BuildOIDSetting("Step1C - Learned MACs By Port MAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4");
            //IList<EnumSNMPOIDIndexType> indexesC = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
            //MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexesC);
            //MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexesC);
            //MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexesC);
            //BuildOIDSetting("StepX - TEST", "1.2.840.10006.300.43", "1.2.840.10006.300.43");

            //------ Otros OID test ------

            //BuildOIDSetting("Step2A", "1.0.8802.1.1.2.1.4.2.1.4", "1.0.8802.1.1.2.1.4.2.1.4");
            //BuildOIDSetting("Step2B", "1.0.8802.1.1.2.1.4.1.1.7", "1.0.8802.1.1.2.1.4.1.1.7");
            //BuildOIDSetting("Step2C", "1.2.840.10006.300.43.1.1.1.1.7", "1.2.840.10006.300.43.1.1.1.1.7");
            //BuildOIDSetting("Step2D", "1.2.840.10006.300.43.1.2.1.1.5", "1.2.840.10006.300.43.1.2.1.1.5");
            //BuildOIDSetting("Step2F", "1.3.6.1.4.1.9.9.46.1.3.1.1.4", "1.3.6.1.4.1.9.9.46.1.3.1.1.4");
            //BuildOIDSetting("Step2G", "1.3.6.1.2.1.17.1.4.1.2", "1.3.6.1.2.1.17.1.4.1.2");
            //BuildOIDSetting("Step2H", "1.3.6.1.2.1.17.2.15.1.3", "1.3.6.1.2.1.17.2.15.1.3");
            //BuildOIDSetting("Step2K", "1.3.6.1.2.1.31.1.1.1.1", "1.3.6.1.2.1.31.1.1.1.1");
            //BuildOIDSetting("Step2L", "1.3.6.1.2.1.31.1.1.1.6", "1.3.6.1.2.1.31.1.1.1.6");
            //BuildOIDSetting("Step2M", "1.3.6.1.2.1.31.1.1.1.10", "1.3.6.1.2.1.31.1.1.1.10");
            //BuildOIDSetting("Step2N", "1.3.6.1.2.1.31.1.1.1.15", "1.3.6.1.2.1.31.1.1.1.15");
            //BuildOIDSetting("Step2Ñ", "1.3.6.1.2.1.31.1.1.1.18", "1.3.6.1.2.1.31.1.1.1.18");

            //1.3.6.1.4.1.11.2 --> nm Evitamos porque es propietario HP...
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

        #region Private Methods

        private void TransformRawData(ISNMPModelDTO Model)
        {
            IDictionary<string, IOIDSettingDTO> OIDSettings = Model.SNMPSettings[RegardingSetting].OIDSettings;

            foreach (ISNMPDeviceDTO Device in Model.SNMPData.Values)
            {
                //Create DTO and attach to device
                ITopologyInfoDTO TopologyInfo = new TopologyInfoDTO();
                Device.AttachSNMPProcessedValue(typeof(ITopologyInfoDTO), TopologyInfo);

                FillBasicInfo(Device, OIDSettings, TopologyInfo); //Fill with basic info
                FillLearnedMACAddresses(Device, OIDSettings, TopologyInfo); //Fill with LearnedAddress inventory
                FillPortMACAddress(Device, OIDSettings, TopologyInfo); //Fill with MAC address of each port
                //FillPortIDInfo(Device, OIDSettings, TopologyInfo); //Fill with port IDs inventory
                
                //Fill with VLAN inventory
                //Fill with DirectNeighbours
            }
        }

        private void BuildTopology(ISNMPModelDTO Model)
        {

        }

        private void FillBasicInfo(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //Get setting of interest
            IOIDSettingDTO SelectedSetting = OIDSettings["DeviceBasicInfo"];

            //Select proper OID entries for processing
            ModelHelper.OIDEntryParser(Device, SelectedSetting, TopologyInfo, BasicInfoHandler);

           

            ////Get setting of interest
            //IOIDSettingDTO SelectedSetting = OIDSettings["DeviceBasicInfo"];

            ////Select proper OID entries for processing
            //IList<ISNMPRawEntryDTO> SelectedData = Device.SNMPRawDataEntries
            //                                    .Where(x => CompareOID(x.Key, SelectedSetting.InitialOID) >= 0 &&
            //                                        (
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) <= 0 && SelectedSetting.InclusiveInterval ||
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) < 0 && !SelectedSetting.InclusiveInterval)
            //                                        )
            //                                    .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
            //                                    .Select(x => x.Value)
            //                                    .ToList();

            ////Iterate on selected data for parsing possible indexes --> Not apliying here

            ////Assign values
            //TopologyInfo.DeviceName = SelectedData[4].ValueData;
            //TopologyInfo.Description = SelectedData[0].ValueData;
            //TopologyInfo.Location = SelectedData[5].ValueData;
            //TopologyInfo.OIDobjectID = SelectedData[1].ValueData;
            ////ToDo...
            //TopologyInfo.OSIImplementedLayers = EnumOSILayers.None;
            //TopologyInfo.DeviceType = EnumDeviceType.None;
        }

        private void BasicInfoHandler(IList<string> IndexData, string ValueData, object TopologyInfoObject)
        {
            ITopologyInfoDTO TopologyInfo = TopologyInfoObject as ITopologyInfoDTO;
            
            //Assign values
            TopologyInfo.DeviceName = SelectedData[4].ValueData;
            TopologyInfo.Description = SelectedData[0].ValueData;
            TopologyInfo.Location = SelectedData[5].ValueData;
            TopologyInfo.OIDobjectID = SelectedData[1].ValueData;
            //ToDo...
            TopologyInfo.OSIImplementedLayers = EnumOSILayers.None;
            TopologyInfo.DeviceType = EnumDeviceType.None;
        }

        private void FillLearnedMACAddresses(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //IDictionary<string, IDictionary<string, string>> LearnedAddresses = new Dictionary<string, IDictionary<string, string>>();

            ////Get setting of interest
            //IOIDSettingDTO SelectedSetting = OIDSettings["LearnedMACByPhysPortID"];

            ////Select proper OID entries for processing
            //IList<ISNMPRawEntryDTO> SelectedData = Device.SNMPRawDataEntries
            //                                    .Where(x => CompareOID(x.Key, SelectedSetting.InitialOID) >= 0 &&
            //                                        (
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) <= 0 && SelectedSetting.InclusiveInterval ||
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) < 0 && !SelectedSetting.InclusiveInterval)
            //                                        )
            //                                    .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
            //                                    .Select(x => x.Value)
            //                                    .ToList();

            ////Iterate on selected data for parsing possible indexes --> Not apliying here
            //foreach (ISNMPRawEntryDTO rawentry in SelectedData)
            //{
            //    //Get matched root OID
            //    //string rootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => x.StartsWith(rawentry.OID)).FirstOrDefault();
            //    string rootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => rawentry.OID.StartsWith(x)).FirstOrDefault();

            //    if (rootOID == null)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        List<int> indexValues = rawentry.OID.Replace(rootOID + ".", "").Split('.').Select(x => int.Parse(x)).ToList();
            //        List<string> indexData = new List<string>();

            //        foreach (EnumSNMPOIDIndexType IndexType in SelectedSetting.IndexedOIDSettings[rootOID].IndexDataDefinitions)
            //        {
            //            switch (IndexType)
            //            {
            //                case EnumSNMPOIDIndexType.Number:
            //                    indexData.Add(indexValues[0].ToString());
            //                    indexValues.RemoveAt(0);

            //                    break;
            //                case EnumSNMPOIDIndexType.MacAddress:
            //                    indexData.Add(string.Join(" ", indexValues.Take(6).Select(x => x.ToString("X"))));
            //                    indexValues.RemoveRange(0, 6);

            //                    break;
            //                case EnumSNMPOIDIndexType.IP:
            //                    break;
            //                case EnumSNMPOIDIndexType.Date:
            //                    break;
            //                case EnumSNMPOIDIndexType.ByteString:
            //                    break;
            //                case EnumSNMPOIDIndexType.Oid:
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }

            //        //Assign values
            //        if (LearnedAddresses.ContainsKey(rawentry.ValueData))
            //        {
            //            if (!LearnedAddresses[rawentry.ValueData].ContainsKey(indexData[1]))
            //            {
            //                LearnedAddresses[rawentry.ValueData].Add(indexData[1], null);
            //            }
            //        }
            //        else
            //        {
            //            LearnedAddresses.Add(rawentry.ValueData, new Dictionary<string, string>() { { indexData[1], null } });
            //        }
            //    }
            //}

            ////Save dictionary on the model
            //TopologyInfo.LearnedAddressByInterfaceID = LearnedAddresses;
        }

        private void FillPortMACAddress(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //IDictionary<string, string> MACByInterface = new Dictionary<string, string>();

            ////Get setting of interest
            //IOIDSettingDTO SelectedSetting = OIDSettings["LearnedMACByPhysPortID"];

            ////Select proper OID entries for processing
            //IList<ISNMPRawEntryDTO> SelectedData = Device.SNMPRawDataEntries
            //                                    .Where(x => CompareOID(x.Key, SelectedSetting.InitialOID) >= 0 &&
            //                                        (
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) <= 0 && SelectedSetting.InclusiveInterval ||
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) < 0 && !SelectedSetting.InclusiveInterval)
            //                                        )
            //                                    .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
            //                                    .Select(x => x.Value)
            //                                    .ToList();

            ////Iterate on selected data for parsing possible indexes --> Not apliying here
            //foreach (ISNMPRawEntryDTO rawentry in SelectedData)
            //{
            //    //Get matched root OID
            //    //string rootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => x.StartsWith(rawentry.OID)).FirstOrDefault();
            //    string rootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => rawentry.OID.StartsWith(x)).FirstOrDefault();

            //    if (rootOID == null)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        List<int> indexValues = rawentry.OID.Replace(rootOID + ".", "").Split('.').Select(x => int.Parse(x)).ToList();
            //        List<string> indexData = new List<string>();

            //        foreach (EnumSNMPOIDIndexType IndexType in SelectedSetting.IndexedOIDSettings[rootOID].IndexDataDefinitions)
            //        {
            //            switch (IndexType)
            //            {
            //                case EnumSNMPOIDIndexType.Number:
            //                    indexData.Add(indexValues[0].ToString());
            //                    indexValues.RemoveAt(0);

            //                    break;
            //                case EnumSNMPOIDIndexType.MacAddress:
            //                    indexData.Add(string.Join(" ", indexValues.Take(6).Select(x => x.ToString("X"))));
            //                    indexValues.RemoveRange(0, 6);

            //                    break;
            //                case EnumSNMPOIDIndexType.IP:
            //                    break;
            //                case EnumSNMPOIDIndexType.Date:
            //                    break;
            //                case EnumSNMPOIDIndexType.ByteString:
            //                    break;
            //                case EnumSNMPOIDIndexType.Oid:
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }

            //        //Assign values
            //        MACByInterface.Add(indexData[0], rawentry.ValueData);
            //    }
            //}

            ////Save dictionary on the model
            //TopologyInfo.MACPortByInterfaceID = MACByInterface;
        }

        private void FillPortIDInfo(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //Dictionary<string, Tuple<EnumPhysPortType, string, string>> PortIDInfo = new Dictionary<string, Tuple<EnumPhysPortType, string, string>>();

            ////Get setting of interest
            //IOIDSettingDTO SelectedSetting = OIDSettings["PhysPortDescription"];

            ////Select proper OID entries for processing
            //IList<ISNMPRawEntryDTO> SelectedData = Device.SNMPRawDataEntries
            //                                    .Where(x => CompareOID(x.Key, SelectedSetting.InitialOID) >= 0 &&
            //                                        (
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) <= 0 && SelectedSetting.InclusiveInterval ||
            //                                            CompareOID(x.Key, SelectedSetting.FinalOID) < 0 && !SelectedSetting.InclusiveInterval)
            //                                        )
            //                                    .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
            //                                    .Select(x => x.Value)
            //                                    .ToList();

            ////Iterate on selected data for parsing possible indexes --> Not apliying here
            //foreach (ISNMPRawEntryDTO rawentry in SelectedData)
            //{
            //    //Get matched root OID
            //    //string rootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => x.StartsWith(rawentry.OID)).FirstOrDefault();
            //    string rootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => rawentry.OID.StartsWith(x)).FirstOrDefault();

            //    if (rootOID == null)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        List<int> indexValues = rawentry.OID.Replace(rootOID + ".", "").Split('.').Select(x => int.Parse(x)).ToList();
            //        List<string> indexData = new List<string>();

            //        foreach (EnumSNMPOIDIndexType IndexType in SelectedSetting.IndexedOIDSettings[rootOID].IndexDataDefinitions)
            //        {
            //            switch (IndexType)
            //            {
            //                case EnumSNMPOIDIndexType.Number:
            //                    indexData.Add(indexValues[0].ToString());
            //                    indexValues.RemoveAt(0);

            //                    break;
            //                case EnumSNMPOIDIndexType.MacAddress:
            //                    indexData.Add(string.Join(" ", indexValues.Take(6).Select(x => x.ToString("X"))));
            //                    indexValues.RemoveRange(0, 6);

            //                    break;
            //                case EnumSNMPOIDIndexType.IP:
            //                    break;
            //                case EnumSNMPOIDIndexType.Date:
            //                    break;
            //                case EnumSNMPOIDIndexType.ByteString:
            //                    break;
            //                case EnumSNMPOIDIndexType.Oid:
            //                    break;
            //                default:
            //                    break;
            //            }

            //            //Assign values
            //            PortIDInfo.Add(indexData[0], new Tuple<EnumPhysPortType, string, string>(EnumPhysPortType.Access, rawentry.ValueData, null));
            //        }
            //    }
            //}

            ////Save dictionary on the model
            //TopologyInfo.PortsDescriptionByInterfaceID = PortIDInfo;
        }

        #endregion
    }
}