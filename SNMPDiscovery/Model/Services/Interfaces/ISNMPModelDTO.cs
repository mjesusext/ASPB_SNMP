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
        IDictionary<string, ISNMPDeviceSettingDTO> SNMPDeviceSettings { get; set; }
        IDictionary<string, ISNMPDeviceDataDTO> SNMPDeviceData { get; set; }
        IDictionary<string, ISNMPProcessedValueDTO> SNMPGlobalProcessedData { get; set; }

        ISNMPDeviceSettingDTO BuildSNMPSetting(string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
        ISNMPDeviceDataDTO BuildSNMPDevice(string targetIPAndMask);
        ISNMPDeviceDataDTO BuildSNMPDevice(int targetIP, int targetMask);
        ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data);

        void StartDiscovery();
        void RunProcesses();
        void SaveData();
    }
}
