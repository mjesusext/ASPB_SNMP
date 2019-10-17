using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    //Leafs of DTO object hierarchy don't need onchange events
    public interface IOIDSettingDTO
    {
        string ID { get; set; }
        string InitialOID { get; set; }
        string FinalOID { get; set; }
        bool InclusiveInterval { get; set; }
        IDictionary<string, IList<EnumSNMPOIDIndexType>> IndexedOIDSettings { get; set; }

        IDictionary<string, IList<EnumSNMPOIDIndexType>> BuildIndexedOIDSetting(string rootOID, IList<EnumSNMPOIDIndexType> indexDataDefs);
    }
}
