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
        private ISNMPView _view { get; set; }
        private ISNMPModelDTO _model { get; set; }

        public SNMPDiscoveryController(ISNMPModelDTO ModelService)
        {
            _model = ModelService;
            _view = new SNMPDiscoveryView(ModelService, this);

            //Test
            _model.Initialize();
            _model.StartDiscovery();
            _model.RunProcesses();
        }

        #region Controller Implementation

        public void GetUserInput(int i)
        {
            //Lanzar metodo de Modelo segun el input introducido y el estado previo si procede
        }

        public IDictionary<int, string> SendCommandsCollection()
        {
            //ToDo
            throw new NotImplementedException();
        }

        #endregion

    }
}
