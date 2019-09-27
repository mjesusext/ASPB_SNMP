using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.Services
{
    public interface ISNMPModelDTO : IObservable<ISNMPModelDTO>, ITrackedObjectContainer
    {
        IDictionary<string, ISNMPSettingDTO> SNMPSettings { get; set; }
        IDictionary<string, ISNMPDeviceDTO> SNMPData { get; set; }

        ISNMPSettingDTO BuildSNMPSetting(string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
        ISNMPDeviceDTO BuildSNMPDevice(string targetIPAndMask);
        ISNMPDeviceDTO BuildSNMPDevice(int targetIP, int targetMask);

        void StartDiscovery();
        void RunProcesses();
        void SaveData();
    }
}
