using SNMPDiscovery.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ITopologyInfoDTO : IDiscoveredBasicInfo
    {
        //Key: Port ID, Value: Port Description (text id)
        IDictionary<string, string> PortsDescriptionByInterfaceID { get; set; }
        //Key: Port ID, Value: Tuple of type and equivalence if needed (for trunks)
        IDictionary<string, CustomPair<EnumPhysPortType, string>> PortsTypologyByInterfaceID { get; set; }
        //Key: Port ID, Value: MAC Address
        IDictionary<string, string> MACPortByInterfaceID { get; set; }
        //Key: Port ID, Value: Tuple of VLAN ID and VLAN name
        IDictionary<string, CustomPair<string, string>> VLANByInterfaceID { get; set; }
        //Key: Port ID, Value: Dictionary of MAC Address - IP Address
        IDictionary<string, IDictionary<string,string>> LearnedAddressByInterfaceID { get; set; }
        //Key: Port ID, Value: Tuple of MAC Address - IP Address
        IDictionary<string, CustomPair<string, string>> DirectNeighboursByInterfaceID { get; set; }
    }
}
