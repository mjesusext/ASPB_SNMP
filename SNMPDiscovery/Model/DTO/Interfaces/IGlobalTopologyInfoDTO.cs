using SNMPDiscovery.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface IGlobalTopologyInfoDTO
    {
        IDictionary<Tuple<string, string, string, string, string>, IList<Tuple<string, string, string, string, string>>> TopologyMatrix { get; set; }
    }
}
