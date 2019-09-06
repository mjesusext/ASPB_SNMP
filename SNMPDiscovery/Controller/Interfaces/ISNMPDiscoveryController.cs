using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Controller
{
    public interface ISNMPDiscoveryController
    {
        IDictionary<int,string> SendCommandsCollection();
        void GetUserInput(int i);
    }
}
