using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;
using SNMPDiscovery.View;
using SNMPDiscovery.Controller.Helpers;

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

        public void DefineDevice(string settingID, string initialIP, string finalIP, string SNMPUser)
        {
            if (string.IsNullOrWhiteSpace(settingID))
            {
                _valMsgs.Add("Null or empty setting ID");
            }

            if (!ControllerHelper.ValidateIPv4Format(initialIP))
            {
                _valMsgs.Add("Invalid initial IP");
            }

            if (!ControllerHelper.ValidateIPv4Format(finalIP))
            {
                _valMsgs.Add("Invalid final IP");
            }

            if (_valMsgs.Count == 0)
            {
                //Consume data
                Model.BuildSNMPSetting(settingID, initialIP, finalIP, SNMPUser);
            }
            else
            {
                OnInvalidInputs(_valMsgs);
                _valMsgs.Clear();
            }
        }

        public void LoadDiscoveryData() { }

        public void DefineProcesses(string settingID, EnumProcessingType processType)
        {
            Model.SNMPSettings[settingID].BuildProcess(processType);
        }

        public void RunDiscovery()
        {
            Model.StartDiscovery();
        }

        public void PullData()
        {

        }

        public void SaveDiscoveryData() { }

        public void SaveProcessedData() { }

        #endregion

    }
}
