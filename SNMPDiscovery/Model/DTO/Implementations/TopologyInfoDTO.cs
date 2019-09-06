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

        public IDictionary<string, Tuple<string, bool>> PortsByInterface { get; set; }
        public IDictionary<string, Tuple<string, string>> VLANByInterface { get; set; }
        public IDictionary<string, string> MACPortByInterface { get; set; }
        public IDictionary<string, IDictionary<string, string>> LearnedAddressByInterface { get; set; }
        public IDictionary<string, Tuple<string,string>> DirectNeighbours { get; set; }
      
    }
}
