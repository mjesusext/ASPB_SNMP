using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class TopologyInfoDTO : ITopologyInfoDTO
    {
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string OIDobjectID { get; set; }
        public EnumOSILayers OSIImplementedLayers { get; set; }
        public EnumDeviceType DeviceType { get; set; }

        public IDictionary<string, string> PortsDescriptionByInterfaceID { get; set; }
        public IDictionary<string, Tuple<EnumPhysPortType, string>> PortsTypologyByInterfaceID { get; set; }
        public IDictionary<string, Tuple<string, string>> VLANByInterfaceID { get; set; }
        public IDictionary<string, string> MACPortByInterfaceID { get; set; }
        public IDictionary<string, IDictionary<string, string>> LearnedAddressByInterfaceID { get; set; }
        public IDictionary<string, Tuple<string,string>> DirectNeighboursByInterfaceID { get; set; }
    }
}
