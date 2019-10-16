using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.View
{
    public enum EnumViewStates : uint
    {
        Main = 0,
        DeviceDefinition = 1,
        LoadDiscoveryData = 2,
        ProcessSelection = 3,
        ProcessExecution = 4,
        PullData = 5,
        SaveDiscoveryData = 6,
        SaveProcessedData = 7,
        BackAction = 8,
        Exit = 9
    }
}
