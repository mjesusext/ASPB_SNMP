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
            Model.SNMPDeviceSettings[settingID].BuildProcess(processType);
        }

        public void RunDiscovery()
        {
            Model.StartDiscovery();
        }

        public void RunProcesses()
        {
            foreach (ISNMPProcessStrategy proccess in Model.SNMPDeviceSettings.Values.SelectMany(x => x.Processes.Values))
            {
                proccess.ValidateInput(Model);
                proccess.Run(Model);
            }
        }

        public object PullDataList(Type dataType, string key = null)
        {
            if (dataType.Equals(typeof(ISNMPDeviceDataDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                            Model.SNMPDeviceData.Values.ToList() : 
                            new List<ISNMPDeviceDataDTO>(new[] { Model.SNMPDeviceData[key] });
            }
            else if (dataType.Equals(typeof(ISNMPDeviceSettingDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                            Model.SNMPDeviceSettings.Values.ToList() : 
                            new List<ISNMPDeviceSettingDTO>(new[] { Model.SNMPDeviceSettings[key] });
            }
            else if (dataType.Equals(typeof(ISNMPProcessStrategy)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                                Model.SNMPDeviceSettings.Values.SelectMany(x => x.Processes.Values).ToList() :
                                new List<ISNMPProcessStrategy>(Model.SNMPDeviceSettings.Values.Select(x => x.Processes[key]));
            }
            else if (dataType.Equals(typeof(IOIDSettingDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                                Model.SNMPDeviceSettings.Values.SelectMany(x => x.OIDSettings.Values).ToList() :
                                new List<IOIDSettingDTO>(Model.SNMPDeviceSettings.Values.Select(x => x.OIDSettings[key]));
            }
            else if (dataType.Equals(typeof(ISNMPRawEntryDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                                Model.SNMPDeviceData.Values.SelectMany(x => x.SNMPRawDataEntries.Values).ToList() : 
                                new List<ISNMPRawEntryDTO>(Model.SNMPDeviceData.Values.Select(x => x.SNMPRawDataEntries[key]));
            }
            else if (dataType.Equals(typeof(ISNMPProcessedValueDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ?
                                Model.SNMPDeviceData.Values.SelectMany(x => x.SNMPProcessedData.Values).ToList() :
                                new List<ISNMPProcessedValueDTO>(Model.SNMPDeviceData.Values.Select(x => x.SNMPProcessedData[key]));
            }
            else
            {
                return null;
            }
        }

        public void SaveDiscoveryData() { }

        public void SaveProcessedData() { }

        #endregion

    }
}
