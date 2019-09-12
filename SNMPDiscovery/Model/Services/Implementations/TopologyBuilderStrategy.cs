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
            //Get setting of interest and number of roots/key OIDs for handling data
            IOIDSettingDTO SelectedSetting = OIDSettings["DeviceBasicInfo"];
            int numRootEntries = SelectedSetting.IndexedOIDSettings.Count;
            List<string> RootEntries = SelectedSetting.IndexedOIDSettings.Keys.ToList();

            //Define handle collection in order
            Action<IList<string>, string, object>[] MappingHandler = new Action<IList<string>, string, object>[numRootEntries];
            MappingHandler[0] = (x, y, z) => { ((ITopologyInfoDTO)z).Description = y; };
            MappingHandler[1] = (x, y, z) => { ((ITopologyInfoDTO)z).OIDobjectID = y; };
            MappingHandler[2] = null;
            MappingHandler[3] = null;
            MappingHandler[4] = (x, y, z) => { ((ITopologyInfoDTO)z).DeviceName = y; };
            MappingHandler[5] = (x, y, z) => { ((ITopologyInfoDTO)z).Location = y; };
            MappingHandler[6] = (x, y, z) => { ((ITopologyInfoDTO)z).Description = y; };

            //Loop of each subset 
            for (int i = 0; i < numRootEntries; i++)
            {
                //1) select OID data subset
                IList<ISNMPRawEntryDTO> SelectedDeviceOID = ModelHelper.OIDDataSelector(Device, RootEntries[i], i+1 == numRootEntries ? RootEntries[i] : RootEntries[i+1]);

                //2) apply specific handle on entryparser
                ModelHelper.OIDEntryParser(SelectedDeviceOID, SelectedSetting.IndexedOIDSettings[RootEntries[i]], TopologyInfo, MappingHandler[i]);
            }
        }

        private void FillLearnedMACAddresses(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //Get setting of interest and number of roots/key OIDs for handling data
            IOIDSettingDTO SelectedSetting = OIDSettings["LearnedMACByPhysPortID"];
            int numRootEntries = SelectedSetting.IndexedOIDSettings.Count;
            List<string> RootEntries = SelectedSetting.IndexedOIDSettings.Keys.ToList();

            //Define handle collection in order
            Action<IList<string>, string, object>[] MappingHandler = new Action<IList<string>, string, object>[numRootEntries];
            MappingHandler[0] = LearnedAddressMapper;

            //Define container if necesary
            TopologyInfo.LearnedAddressByInterfaceID = new Dictionary<string, IDictionary<string, string>>();

            //Loop of each subset 
            for (int i = 0; i < numRootEntries; i++)
            {
                //1) select OID data subset
                IList<ISNMPRawEntryDTO> SelectedDeviceOID = ModelHelper.OIDDataSelector(Device, RootEntries[i], i + 1 == numRootEntries ? RootEntries[i] : RootEntries[i + 1]);

                //2) apply specific handle on entryparser
                ModelHelper.OIDEntryParser(SelectedDeviceOID, SelectedSetting.IndexedOIDSettings[RootEntries[i]], TopologyInfo, MappingHandler[i]);
            }
        }

        private void LearnedAddressMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            ITopologyInfoDTO TopologyInfo = StrategyDTOobject as ITopologyInfoDTO;

            IDictionary<string, IDictionary<string, string>> LearnedAddres = TopologyInfo.LearnedAddressByInterfaceID;

            if (LearnedAddres.ContainsKey(Value))
            {
                if (!LearnedAddres[Value].ContainsKey(IndexValues[1]))
                {
                    LearnedAddres[Value].Add(IndexValues[1], null);
                }
            }
            else
            {
                LearnedAddres.Add(Value, new Dictionary<string, string>() { { IndexValues[1], null } });
            }
        }

        private void FillPortMACAddress(ISNMPDeviceDTO Device, IDictionary<string, IOIDSettingDTO> OIDSettings, ITopologyInfoDTO TopologyInfo)
        {
            //Get setting of interest and number of roots/key OIDs for handling data
            IOIDSettingDTO SelectedSetting = OIDSettings["PhysPortMACAddress"];
            int numRootEntries = SelectedSetting.IndexedOIDSettings.Count;
            List<string> RootEntries = SelectedSetting.IndexedOIDSettings.Keys.ToList();

            //Define handle collection in order
            Action<IList<string>, string, object>[] MappingHandler = new Action<IList<string>, string, object>[numRootEntries];
            MappingHandler[0] = PortMACAddressMapper;

            //Define container if necesary
            TopologyInfo.MACPortByInterfaceID = new Dictionary<string, string>();

            //Loop of each subset 
            for (int i = 0; i < numRootEntries; i++)
            {
                //1) select OID data subset
                IList<ISNMPRawEntryDTO> SelectedDeviceOID = ModelHelper.OIDDataSelector(Device, RootEntries[i], i + 1 == numRootEntries ? RootEntries[i] : RootEntries[i + 1]);

                //2) apply specific handle on entryparser
                ModelHelper.OIDEntryParser(SelectedDeviceOID, SelectedSetting.IndexedOIDSettings[RootEntries[i]], TopologyInfo, MappingHandler[i]);
            }
        }

        private void PortMACAddressMapper(IList<string> IndexValues, string Value, object StrategyDTOobject)
        {
            ITopologyInfoDTO TopologyInfo = StrategyDTOobject as ITopologyInfoDTO;

            IDictionary<string, string> MACofPorts = TopologyInfo.MACPortByInterfaceID;

            MACofPorts.Add(IndexValues[0], Value);
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