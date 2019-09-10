using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ITopologyInfoDTO : IDiscoveredBasicInfo
    {
        IDictionary<string, string> PortsDescriptionByInterfaceID { get; set; }
        IDictionary<string, Tuple<EnumPhysPortType, string>> PortsTypologyByInterfaceID { get; set; } 
        IDictionary<string, string> MACPortByInterfaceID { get; set; }
        IDictionary<string, Tuple<string, string>> VLANByInterfaceID { get; set; }
        IDictionary<string, IDictionary<string,string>> LearnedAddressByInterfaceID { get; set; }
        IDictionary<string, Tuple<string, string>> DirectNeighboursByInterfaceID { get; set; }
    }
}
