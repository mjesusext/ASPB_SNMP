using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ITopologyInfoDTO : IDiscoveredBasicInfo
    {
        IDictionary<string, Tuple<string,bool>> PortsByInterface { get; set; }
        IDictionary<string, string> MACPortByInterface { get; set; }
        IDictionary<string, Tuple<string, string>> VLANByInterface { get; set; }
        IDictionary<string, IDictionary<string,string>> LearnedAddressByInterface { get; set; }
        IDictionary<string, Tuple<string, string>> DirectNeighbours { get; set; }
    }
}
