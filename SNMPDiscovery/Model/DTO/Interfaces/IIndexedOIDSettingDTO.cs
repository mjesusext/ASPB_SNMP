using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface IIndexedOIDSettingDTO
    {
        string RootOID { get; set; }
        IList<EnumSNMPOIDIndexType> IndexDataDefinitions { get; set; }
    }
}