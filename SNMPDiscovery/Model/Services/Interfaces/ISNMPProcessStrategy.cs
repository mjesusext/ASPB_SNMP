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
        EnumProcessingType ProcessID { get; }
        ISNMPModelDTO RegardingObject { get; set; }
        IList<ISNMPDeviceSettingDTO> TargetDevices { get; set; }
        IDictionary<string, IOIDSettingDTO> OIDSettings { get; set; }

        void BuildOIDSetting();
        void Run();
    }
}
