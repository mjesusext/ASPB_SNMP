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
            this._model = ModelService;
            this._view = new SNMPDiscoveryView(ModelService, this);
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
