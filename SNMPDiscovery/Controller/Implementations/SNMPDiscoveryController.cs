using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;
using SNMPDiscovery.View;

namespace SNMPDiscovery.Controller
{
    public class SNMPDiscoveryController : ISNMPDiscoveryController
    {
        public IObserver<ISNMPModelDTO> _view { get; set; }
        public ISNMPModelDTO _model { get; set; }

        public SNMPDiscoveryController(ISNMPModelDTO ModelService)
        {
            _model = ModelService;
            _view = new SNMPDiscoveryView(ModelService, this);

            //Test
            _model.Initialize();
        }

        #region Controller Implementation
        
        public void DefineDevice(string settingID, string initialIP, string finalIP, string SNMPUser)
        {
            _model.BuildSNMPSetting(settingID, initialIP, finalIP, SNMPUser);
        }

        public void LoadDiscoveryData() { }

        public void DefineProcesses(string settingID, EnumProcessingType processType)
        {
            _model.SNMPSettings[settingID].BuildProcess(processType);
        }

        public void RunDiscovery()
        {
            _model.StartDiscovery();
        }

        public void PullData()
        {

        }

        public void SaveDiscoveryData() { }

        public void SaveProcessedData() { }

        #endregion

    }
}
