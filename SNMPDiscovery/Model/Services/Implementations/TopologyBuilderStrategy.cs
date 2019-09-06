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

                TopologyInfo.PortsByInterface.Add(DescriptionEntry.Value.OID.Replace(DescriptionEntry.Value.RootOID + "." , ""), new Tuple<string, bool>(DescriptionEntry.Value.ValueData, false));
            }

            int i = 3;
        }

        #endregion
    }
}
