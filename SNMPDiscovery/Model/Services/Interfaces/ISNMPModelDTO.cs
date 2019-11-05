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
        IDictionary<EnumProcessingType, ISNMPProcessStrategy> Processes { get; set; }
        IDictionary<string, ISNMPDeviceDataDTO> DeviceData { get; set; }
        IDictionary<string, ISNMPProcessedValueDTO> GlobalProcessedData { get; set; }
        IDictionary<string, string> ARPTable { get; set; }

        ISNMPDeviceSettingDTO BuildSNMPSetting(string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
        ISNMPDeviceSettingDTO EditSNMPSetting(string oldID, string ID, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
        void DeleteSNMPSetting(string ID);
        ISNMPProcessStrategy BuildProcess(string SettingID, EnumProcessingType ProcessType);
        ISNMPProcessStrategy EditProcess(EnumProcessingType PreviousProcessType, EnumProcessingType NewProcessType);
        void DeleteProcess(EnumProcessingType ProcessType);
        ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data);

        void StartDiscovery();
        void RunProcesses();
        void SaveData();
    }
}
