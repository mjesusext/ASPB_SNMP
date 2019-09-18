using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.Services
{
    public interface ISNMPProcessStrategy : ITrackeableObject
    {
        string ProcessID { get; }
        string RegardingSetting { get; set; }

        IDictionary<string, IOIDSettingDTO> BuildOIDSetting(string regardingSetting, IDictionary<string, IOIDSettingDTO> OIDSettings);
        void ValidateInput(ISNMPModelDTO Model);
        void Run(ISNMPModelDTO Model);
    }
}
