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
        None = 0,
        Hub = 1 << 0,
        Bridge = 1 << 2,
        Switch = 1 << 3,
        Router = 1 << 4,
        Firewall = 1 << 5,
        AP = 1 << 6,
        Printer = 1 << 7,
        Server = 1 << 8,
        DiscRack = 1 << 9,
        VirtualMachine = 1 << 10,
        Host = 1 << 11
    }
}
