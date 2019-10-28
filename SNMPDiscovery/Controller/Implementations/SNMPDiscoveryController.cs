using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;
using SNMPDiscovery.View;
using SNMPDiscovery.Model.Helpers;

namespace SNMPDiscovery.Controller
{
    public class SNMPDiscoveryController : ISNMPDiscoveryController
    {
        public IObserver<ISNMPModelDTO> View { get; set; }
        public ISNMPModelDTO Model { get; set; }
        public event Action<List<string>> OnInvalidInputs;

        private List<string> _valMsgs;

        public SNMPDiscoveryController(ISNMPModelDTO ModelService)
        {
            _valMsgs = new List<string>();
            Model = ModelService;
            View = new SNMPDiscoveryView(ModelService, this);
        }

        #region Controller Implementation

        public void DefineDevices(string settingID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            if (string.IsNullOrWhiteSpace(settingID))
            {
                _valMsgs.Add("Null or empty setting ID");
            }

            if (!ModelHelper.ValidateIPAndMask(initialIPAndMask))
            {
                _valMsgs.Add("Invalid initial IP");
            }

            if (!ModelHelper.ValidateIPAndMask(finalIPAndMask))
            {
                _valMsgs.Add("Invalid final IP");
            }

            if(!ModelHelper.ValidateIPandMaskRange(initialIPAndMask, finalIPAndMask))
            {
                _valMsgs.Add("Invalid IP range");
            }

            if (_valMsgs.Count == 0)
            {
                //Consume data
                Model.BuildSNMPSetting(settingID, initialIPAndMask, finalIPAndMask, SNMPUser);
            }
            else
            {
                OnInvalidInputs(_valMsgs);
                _valMsgs.Clear();
            }
        }

        public void LoadDiscoveryData() { }

        public void LoadDeviceDefinitions() { }

        public void DefineProcesses(string settingID, EnumProcessingType processType)
        {
            Model.BuildProcess(settingID, processType);
        }

        public void RunDiscovery()
        {
            Model.StartDiscovery();
        }

        public void RunProcesses()
        {
            foreach (ISNMPProcessStrategy proccess in Model.Processes.Values)
            {
                proccess.Run(Model);
            }
        }

        public object PullDataList(Type dataType, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (dataType.Equals(typeof(ISNMPDeviceDataDTO)))
                {
                    return Model.DeviceData.Values.ToList();
                }
                else if (dataType.Equals(typeof(ISNMPDeviceSettingDTO)))
                {
                    return Model.DeviceSettings.Values.ToList();
                }
                else if (dataType.Equals(typeof(ISNMPProcessStrategy)))
                {
                    return Model.Processes.Values.ToList();
                }
                else if (dataType.Equals(typeof(IOIDSettingDTO)))
                {
                    return Model.Processes.Values.SelectMany(x => x.OIDSettings.Values).ToList();
                }
                else if (dataType.Equals(typeof(ISNMPRawEntryDTO)))
                {
                    return Model.DeviceData.Values.SelectMany(x => x.SNMPRawDataEntries.Values).ToList();
                }
                else if (dataType.Equals(typeof(ISNMPProcessedValueDTO)))
                {
                    return Model.DeviceData.Values.SelectMany(x => x.SNMPProcessedData.Values).ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (dataType.Equals(typeof(ISNMPDeviceDataDTO)))
                {
                    return new List<ISNMPDeviceDataDTO>(new[] { Model.DeviceData[key] });
                }
                else if (dataType.Equals(typeof(ISNMPDeviceSettingDTO)))
                {
                    return new List<ISNMPDeviceSettingDTO>(new[] { Model.DeviceSettings[key] });
                }
                else if (dataType.Equals(typeof(ISNMPProcessStrategy)))
                {
                    return new List<ISNMPProcessStrategy>(new[] { Model.Processes[key] });
                }
                else if (dataType.Equals(typeof(IOIDSettingDTO)))
                {
                    return new List<IOIDSettingDTO>(Model.Processes.Values.Select(x => x.OIDSettings[key]));
                }
                else if (dataType.Equals(typeof(ISNMPRawEntryDTO)))
                {
                    return new List<ISNMPRawEntryDTO>(Model.DeviceData.Values.Select(x => x.SNMPRawDataEntries[key]));
                }
                else if (dataType.Equals(typeof(ISNMPProcessedValueDTO)))
                {
                    return new List<ISNMPProcessedValueDTO>(Model.DeviceData.Values.Select(x => x.SNMPProcessedData[key]));
                }
                else
                {
                    return null;
                }
            }
        }

        public void SaveDiscoveryData() { }

        public void SaveProcessedData() { }

        #endregion

    }
}
