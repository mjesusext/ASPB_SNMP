using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface IOIDSettingDTO
    {
        string ID { get; set; }
        string InitialOID { get; set; }
        string FinalOID { get; set; }
        bool InclusiveInterval { get; set; }
        IDictionary<string, IIndexedOIDSettingDTO> IndexedOIDSettings { get; set; }

        IIndexedOIDSettingDTO BuildIndexedOIDSetting(string rootOID, IList<EnumSNMPOIDIndexType> indexDataDefs);
    }
}
