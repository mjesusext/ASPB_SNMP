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

        ISNMPSettingDTO BuildSNMPSetting(string ID, string initialIP, string finalIP, string SNMPUser);
        ISNMPDeviceDTO BuildSNMPDevice(string targetIP);
        ISNMPDeviceDTO BuildSNMPDevice(int targetIP);

        void Initialize();
        void StartDiscovery();
        void RunProcesses();
        void SaveData();
    }
}
