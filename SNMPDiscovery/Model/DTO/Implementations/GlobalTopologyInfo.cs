using SNMPDiscovery.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class GlobalTopologyInfo : IGlobalTopologyInfoDTO
    {
        public IDictionary<Tuple<string, string, string, string, string>, IList<Tuple<string, string, string, string, string>>> TopologyMatrix { get; set; }

        public GlobalTopologyInfo()
        {
            TopologyMatrix = new Dictionary<Tuple<string, string, string, string, string>, IList<Tuple<string, string, string, string, string>>>();
        }
    }
}
