using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPRawEntryDTO : ITrackeableObject
    {
        string OID { get; set; }
        string ValueData { get; set; }
        EnumSNMPOIDType DataType { get; set; }
    }
}
