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
        private IDisposable _observeableSubscription { get; set; }

        public SNMPDiscoveryView(ISNMPModelDTO Model, ISNMPDiscoveryController Controller)
        {
            _controller = Controller;
            
            //_observeableSubscriptions.Add()
            //    IObserver<ISNMPDeviceDTO>, 
            //    IObserver<ISNMPSettingDTO>,
            //    IObserver<ISNMPProcessStrategy>,
            //    IObserver<IOIDSettingDTO>,
            //    IObserver<ISNMPRawEntryDTO>,
            //    IObserver<ISNMPProcessedValueDTO>

            Initialize();
        }

        #region Observable Implementation

        public void OnNext(ISNMPModelDTO value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
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
