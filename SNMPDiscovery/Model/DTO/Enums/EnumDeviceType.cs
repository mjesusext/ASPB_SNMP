using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    [Flags]
    public enum EnumDeviceType : uint
    {
        Unknown = 0,
        HB = 1 << 0,
        BD = 1 << 2,
        SW = 1 << 3,
        RT = 1 << 4,
        FW = 1 << 5,
        AP = 1 << 6,
        PRT = 1 << 7,
        SRV = 1 << 8,
        CAB = 1 << 9,
        VM = 1 << 10,
        HST = 1 << 11
    }
}
