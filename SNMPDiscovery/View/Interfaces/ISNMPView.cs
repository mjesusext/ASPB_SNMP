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
        void ShowData(ISNMPDeviceDTO data);
        void ShowData(ISNMPSettingDTO data);
        void ShowData(ISNMPProcessStrategy data);
        void ShowData(IOIDSettingDTO data);
        void ShowData(ISNMPRawEntryDTO data);
        void ShowData(ISNMPProcessedValueDTO data);
    }
}
