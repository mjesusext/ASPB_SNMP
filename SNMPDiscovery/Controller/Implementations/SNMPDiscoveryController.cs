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

        public void DefineDevice(string settingID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
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
            Model.SNMPSettings[settingID].BuildProcess(processType);
        }

        public void RunDiscovery()
        {
            Model.StartDiscovery();
        }

        public void RunProcesses()
        {
            foreach (ISNMPProcessStrategy proccess in Model.SNMPSettings.Values.SelectMany(x => x.Processes.Values))
            {
                proccess.ValidateInput(Model);
                proccess.Run(Model);
            }
        }

        public object PullDataList(Type dataType, string key = null)
        {
            if (dataType.Equals(typeof(ISNMPDeviceDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                            Model.SNMPData.Values.ToList() : 
                            new List<ISNMPDeviceDTO>(new[] { Model.SNMPData[key] });
            }
            else if (dataType.Equals(typeof(ISNMPSettingDTO)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                            Model.SNMPSettings.Values.ToList() : 
                            new List<ISNMPSettingDTO>(new[] { Model.SNMPSettings[key] });
            }
            else if (dataType.Equals(typeof(ISNMPProcessStrategy)))
            {
                return string.IsNullOrWhiteSpace(key) ? 
                                Model.SNMPSettings.Values.SelectMany(x => x.Processes.Values).ToList() :
                                new List<ISNMPProcessStrategy>(Model.SNMPSettings.Values.Select(x => x.Processes[key]));
            }
            else if (dataType.Equals(typeof(IOIDSettingDTO)))
            {
                return null;
            }
            else if (dataType.Equals(typeof(ISNMPRawEntryDTO)))
            {
                return null;
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
