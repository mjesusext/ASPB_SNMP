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
