using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.Services
{
    public interface ISNMPProcessStrategy
    {
        string ID { get; set; }

        IDictionary<string, IOIDSettingDTO> BuildOIDSetting(IDictionary<string, IOIDSettingDTO> OIDSettings);
        void ValidateInput(IDictionary<string, ISNMPDeviceDTO> DevicesData);
        void Run(IDictionary<string, ISNMPDeviceDTO> DevicesData);
    }
}
