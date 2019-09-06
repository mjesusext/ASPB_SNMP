using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.Services
{
    public class TopologyBuilderStrategy : ISNMPProcessStrategy
    {
        public string ID { get; set; } = "TopologyBuilder";

        public IDictionary<string, IOIDSettingDTO> BuildOIDSetting(IDictionary<string, IOIDSettingDTO> OIDSettings)
        {
            //Lazy initialization
            if (OIDSettings == null)
            {
                OIDSettings = new Dictionary<string, IOIDSettingDTO>();
            }

            //ToDo
            //1 - Check if exists. TRUE = ADD, FALSE = SKIP
            //IOIDSettingDTO setting = new OIDSettingDTO(id, initialOID, finalOID, initialOID == finalOID);
            //OIDSettings.Add(id, setting);

            if (!OIDSettings.ContainsKey("Step1A - Basic Info"))
            {
                OIDSettings.Add("Step1A - Basic Info", new OIDSettingDTO("Step1A - Basic Info", "1.3.6.1.2.1.1.1", "1.3.6.1.2.1.1.8", false));
            }

            if (!OIDSettings.ContainsKey("Step2E - Port descriptive names"))
            {
                IOIDSettingDTO MockOIDSettingB = new OIDSettingDTO("Step2E - Port descriptive names", "1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.2", true);
                IList<EnumSNMPOIDIndexType> indexesB = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.Number };
                MockOIDSettingB.BuildIndexedOIDSetting("1.3.6.1.2.1.2.2.1.2", indexesB);

                OIDSettings.Add("Step2E - Port descriptive names", MockOIDSettingB);
            }

            if (!OIDSettings.ContainsKey("Step1B - Port MAC Address"))
            {
                OIDSettings.Add("Step1B - Port MAC Address", new OIDSettingDTO("Step1B - Port MAC Address", "1.3.6.1.2.1.2.2.1.6", "1.3.6.1.2.1.2.2.1.6", true));
            }

            if (!OIDSettings.ContainsKey("Step2J - Learned MAC Address By Port ID"))
            {
                OIDSettings.Add("Step2J - Learned MAC Address By Port ID", new OIDSettingDTO("Step2J - Learned MAC Address By Port ID", "1.3.6.1.2.1.17.7.1.2.2.1.2", "1.3.6.1.2.1.17.7.1.2.2.1.2", true));
            }

            if (!OIDSettings.ContainsKey("Step1C - Learned MACs By Port MAC"))
            {
                IOIDSettingDTO MockOIDSettingC = new OIDSettingDTO("Step1C - Learned MACs By Port MAC", "1.3.6.1.2.1.17.4.3.1", "1.3.6.1.2.1.17.4.3.4", false);
                IList<EnumSNMPOIDIndexType> indexesC = new List<EnumSNMPOIDIndexType>() { EnumSNMPOIDIndexType.MacAddress };
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.1", indexesC);
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.2", indexesC);
                MockOIDSettingC.BuildIndexedOIDSetting("1.3.6.1.2.1.17.4.3.1.3", indexesC);

                OIDSettings.Add("Step1C - Learned MACs By Port MAC", MockOIDSettingC);
            }

            if (!OIDSettings.ContainsKey("StepX - TEST"))
            {
                OIDSettings.Add("StepX - TEST", new OIDSettingDTO("StepX - TEST", "1.2.840.10006.300.43", "1.2.840.10006.300.43", true));
            }

            #region Version antigua

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

            //------ Otros OID test ------

            //BuildOIDSetting("StepX - TEST", "1.2.840.10006.300.43", "1.2.840.10006.300.43");

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

        public void Run(IDictionary<string, ISNMPDeviceDTO> DevicesData)
        {
            TransformRawData(DevicesData);
            BuildTopology(DevicesData);
        }

        public void ValidateInput(IDictionary<string, ISNMPDeviceDTO> DevicesData)
        {
            bool[] valResults = new bool[5];
            //Check if the exists entries of specified OID ranges for every Device

            foreach (ISNMPDeviceDTO Device in DevicesData.Values)
            {
                valResults[0] = Device.SNMPRawDataEntries.Any(x => x.Key.StartsWith("1.3.6.1.2.1.1.1")); //Basic info
                valResults[1] = Device.SNMPRawDataEntries.Any(x => x.Key.StartsWith("1.3.6.1.2.1.2.2.1.2")); //Port descriptive names
                valResults[2] = Device.SNMPRawDataEntries.Any(x => x.Key.StartsWith("1.3.6.1.2.1.2.2.1.6")); //Port MAC Address
                valResults[3] = Device.SNMPRawDataEntries.Any(x => x.Key.StartsWith("1.3.6.1.2.1.17.7.1.2.2.1.2")); //Learned MAC Address by port ID
                valResults[4] = Device.SNMPRawDataEntries.Any(x => x.Key.StartsWith("1.3.6.1.2.1.17.7.1.4.3.1")); //VLAN detection by port (except Trunks)

                if(valResults.All(x => x == true))
                {
                    //Test
                    Console.WriteLine($"Device with IP {Device.TargetIP.ToString()} contains requiered OIDs");
                }
                else
                {
                    //Test
                    Console.WriteLine($"Device with IP {Device.TargetIP.ToString()} does not contains requiered OIDs");
                }
            }
        }

        #region Private Methods

        private void TransformRawData(IDictionary<string, ISNMPDeviceDTO> DevicesData)
        {
            foreach(ISNMPDeviceDTO Device in DevicesData.Values)
            {
                //Create DTO and attach to device
                ITopologyInfoDTO TopologyInfo = new TopologyInfoDTO();
                Device.AttachSNMPProcessedValue(typeof(ITopologyInfoDTO), TopologyInfo);

                //Fill with basic info
                FillBasicInfo(Device, TopologyInfo);

                //Fill with port IDs inventory
                FillPortIDInfo(Device, TopologyInfo);
                //Fill with port MAC inventory
                //Fill with VLAN inventory
                //Fill with LearnedAddress inventory
                //Fill with DirectNeighbours
            }
        }

        private void BuildTopology(IDictionary<string, ISNMPDeviceDTO> DevicesData)
        {

        }

        private void FillBasicInfo(ISNMPDeviceDTO Device, ITopologyInfoDTO TopologyInfo)
        {
            TopologyInfo.DeviceName = Device.SNMPRawDataEntries["1.3.6.1.2.1.1.5.0"].ValueData;
            TopologyInfo.Description = Device.SNMPRawDataEntries["1.3.6.1.2.1.1.1.0"].ValueData;
            TopologyInfo.Location = Device.SNMPRawDataEntries["1.3.6.1.2.1.1.6.0"].ValueData;
            TopologyInfo.OIDobjectID = Device.SNMPRawDataEntries["1.3.6.1.2.1.1.2.0"].ValueData;
            //ToDo...
            TopologyInfo.OSIImplementedLayers = EnumOSILayers.None;
            TopologyInfo.DeviceType = EnumDeviceType.None;
        }

        private void FillPortIDInfo(ISNMPDeviceDTO Device, ITopologyInfoDTO TopologyInfo)
        {
            TopologyInfo.PortsByInterface = new Dictionary<string, Tuple<string, bool>>();

            IEnumerable<KeyValuePair<string,ISNMPRawEntryDTO>> PortDescriptions = Device.SNMPRawDataEntries.Where(x => x.Key.StartsWith("1.3.6.1.2.1.2.2.1.2."));

            foreach (var DescriptionEntry in PortDescriptions)
            {

                //TopologyInfo.PortsByInterface.Add(DescriptionEntry.Value.OID.Replace(DescriptionEntry.Value.RootOID + "." , ""), new Tuple<string, bool>(DescriptionEntry.Value.ValueData, false));
            }
        }

        #endregion
    }
}
