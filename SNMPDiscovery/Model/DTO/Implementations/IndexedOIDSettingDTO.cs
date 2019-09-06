using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class IndexedOIDSettingDTO : IIndexedOIDSettingDTO
    {
        public string RootOID { get; set; }
        public IList<EnumSNMPOIDIndexType> IndexDataDefinitions { get; set; }

        public IndexedOIDSettingDTO()
        {
        }

        public IndexedOIDSettingDTO(string rootoid, IList<EnumSNMPOIDIndexType> dataTypes)
        {
            RootOID = rootoid;
            IndexDataDefinitions = dataTypes;
        }
    }
}
