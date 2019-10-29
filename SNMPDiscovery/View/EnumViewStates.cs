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
        AddDeviceDefinition = 1,
        LoadDiscoveryProfile = 2,
        AddProcessDefinition = 3,
        ShowDeviceDefinitions = 4,
        EditDeviceDefinition = 5,
        DeleteDeviceDefinition = 6,
        ShowProcessDefinitions = 7,
        EditProcessDefinition = 8,
        DeleteProcessDefinition = 9,
        RunProcess = 10,
        DataSearch = 11,
        SaveDiscoveryData = 12,
        SaveProcessedData = 13,
        BackAction = 14,
        Exit = 15
    }
}
