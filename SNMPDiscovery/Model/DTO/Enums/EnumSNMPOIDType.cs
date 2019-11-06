using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public enum EnumSNMPOIDType : uint
    {
        Counter64 = 0,
        Integer32 = 1,
        MsgFlags = 2,
        Null = 3, 
        OctetString = 4,
        Oid = 5,
        Pdu = 6,
        Sequence = 7,
        TrapPdu = 8,
        UInteger32 = 9,
        UserSecurityModel = 10,
        V2Error = 11,
        Vb = 12,
        VbCollection = 13,
        ObjectId = 14,
        TimeTicks = 15,
        Gauge32 = 16,
        Counter32 = 17,
        Unknown = 18
    }
}
