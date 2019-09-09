using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPModelDTO
    {
        IDictionary<string, ISNMPSettingDTO> SNMPSettings { get; set; }
        IDictionary<string, ISNMPDeviceDTO> SNMPData { get; set; }

        ISNMPSettingDTO BuildSNMPSetting(string ID, string initialIP, string finalIP, string SNMPUser);
        ISNMPDeviceDTO BuildSNMPDevice(string targetIP);
        ISNMPDeviceDTO BuildSNMPDevice(int targetIP);
    }
}
