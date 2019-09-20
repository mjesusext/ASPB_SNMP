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
        NotificationSetting = 5,
        PullData = 6,
        SaveDiscoveryData = 7,
        SaveProcessedData = 8,
        BackMenu = 9,
        Exit = 10
    }
}
