using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public enum EnumSNMPOIDIndexType : uint
    {
        Number = 0,
        MacAddress = 1,
        IP = 2,
        Date = 3,
        ByteString = 4,
        Oid = 5
    }
}
