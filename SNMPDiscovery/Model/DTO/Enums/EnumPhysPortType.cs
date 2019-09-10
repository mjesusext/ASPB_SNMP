using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public enum EnumPhysPortType : uint
    {
        Loopback = 0,
        Access = 1,
        Trunk = 2,
        InferedTrunk = 3,
        LACP = 4,
        Aggregate = 5
    }
}
