using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface IDiscoveredBasicInfo
    {
        string DeviceIPAndMask { get; set; }
        string DeviceMAC { get; set; }
        string DeviceName { get; set; }
        string Description { get; set; }
        string Location { get; set; }
        string OIDobjectID { get; set; }
        EnumOSILayers OSIImplementedLayers { get; set; }
        EnumDeviceType DeviceType { get; set; }
    }
}