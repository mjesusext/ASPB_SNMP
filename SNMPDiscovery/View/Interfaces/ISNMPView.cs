using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.View
{
    public interface ISNMPView : IObserver<ISNMPModelDTO>
    {
        void Initialize();
        void ShowCommands();
        void ShowSNMPDevice(ISNMPModelDTO Model);
        void ShowSNMPSetting(ISNMPSettingDTO Model);
    }
}
