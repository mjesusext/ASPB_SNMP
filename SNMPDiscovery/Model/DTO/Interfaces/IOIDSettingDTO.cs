using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface IOIDSettingDTO : IObservable<IOIDSettingDTO>
    {
        string ID { get; set; }
        string InitialOID { get; set; }
        string FinalOID { get; set; }
        bool InclusiveInterval { get; set; }
        IDictionary<string, IList<EnumSNMPOIDIndexType>> IndexedOIDSettings { get; set; }

        IDictionary<string, IList<EnumSNMPOIDIndexType>> BuildIndexedOIDSetting(string rootOID, IList<EnumSNMPOIDIndexType> indexDataDefs);
    }
}
