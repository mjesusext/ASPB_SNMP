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
        IDictionary<string, ISNMPDeviceSettingDTO> DeviceSettings { get; set; }
        IDictionary<string, ISNMPProcessStrategy> Processes { get; set; }
        IDictionary<string, ISNMPDeviceDataDTO> DeviceData { get; set; }
        IDictionary<string, ISNMPProcessedValueDTO> GlobalProcessedData { get; set; }

        ISNMPDeviceSettingDTO BuildSNMPSetting(string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
        ISNMPProcessStrategy BuildProcess(string SettingID, EnumProcessingType ProcessType);
        ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data);

        void StartDiscovery();
        void RunProcesses();
        void SaveData();
    }
}
