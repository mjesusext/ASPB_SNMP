using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    [Flags]
    public enum EnumOSILayers : uint
    {
        Unknown = 0,
        Physical = 1 << 0,
        DataLink = 1 << 1,
        Network = 1 << 2,
        Transport = 1 << 3,
        Session = 1 << 4,
        Presentation = 1 << 5,
        Application = 1 << 6
    }
}
