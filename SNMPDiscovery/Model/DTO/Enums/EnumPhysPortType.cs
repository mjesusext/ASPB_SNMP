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
        LACP = 3,
        Aggregate = 4,
        VirtualPort = 5,
        InferedTrunk = 6
    }
}
