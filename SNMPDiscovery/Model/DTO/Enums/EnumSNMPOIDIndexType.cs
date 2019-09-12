using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public enum EnumSNMPOIDIndexType : uint
    {
        None = 0,
        Number = 1,
        MacAddress = 2,
        IP = 3,
        Date = 4,
        ByteString = 5,
        Oid = 6
    }
}
