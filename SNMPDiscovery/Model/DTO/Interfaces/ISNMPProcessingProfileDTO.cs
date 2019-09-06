using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPProcessingProfileDTO
    {
        string ID { get; set; }
        ISNMPProcessStrategy Process { get; set; }
        IDictionary<string, IOIDSettingDTO> OIDSettingsCollection { get; set; }

        IOIDSettingDTO BuildOIDSetting(string id, string initialOID, string finalOID);
    }
}
