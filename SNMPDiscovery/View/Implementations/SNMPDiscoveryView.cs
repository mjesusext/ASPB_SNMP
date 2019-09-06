using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;
using SNMPDiscovery.Controller;

namespace SNMPDiscovery.View
{
    public class SNMPDiscoveryView : ISNMPView
    {
        private ISNMPDiscoveryController _controller { get; set; }
        private ISNMPModelService _modelService { get; set; }
        private IDisposable _subscription { get; set; }

        public SNMPDiscoveryView(ISNMPModelService ModelService, ISNMPDiscoveryController Controller)
        {
            this._modelService = ModelService;
            this._controller = Controller;
            _subscription = _modelService.Subscribe(this);

            Initialize();
        }

        #region Observer Implementation
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ISNMPModelDTO value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region View Implementation
        public void Initialize()
        {
            Console.WriteLine("Inicializando herramienta de documentación de red.");
            Console.ReadKey();
        }

        public void ShowCommands()
        {
            throw new NotImplementedException();
        }

        public void ShowSNMPDevice(ISNMPModelDTO Model)
        {
            throw new NotImplementedException();
        }

        public void ShowSNMPSetting(ISNMPSettingDTO Model)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
