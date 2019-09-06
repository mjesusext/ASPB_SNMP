using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPProcessingProfile : ISNMPProcessingProfileDTO
    {
        public string ID { get; set; }
        public ISNMPProcessStrategy Process { get; set; }
        public IDictionary<string, IOIDSettingDTO> OIDSettingsCollection { get; set; }

        public SNMPProcessingProfile()
        {
        }

        public SNMPProcessingProfile(string id, EnumProcessingType ProcessType)
        {
            switch (ProcessType)
            {
                case EnumProcessingType.None:
                    break;
                case EnumProcessingType.TopologyDiscovery:
                    Process = new TopologyBuilderStrategy();
                    break;
                case EnumProcessingType.PrinterConsumption:
                    break;
                default:
                    break;
            }
        }

        public IOIDSettingDTO BuildOIDSetting(string id, string initialOID, string finalOID)
        {
            //Lazy initialization
            if (OIDSettingsCollection == null)
            {
                OIDSettingsCollection = new Dictionary<string, IOIDSettingDTO>();
            }

            IOIDSettingDTO setting = new OIDSettingDTO(id, initialOID, finalOID, initialOID == finalOID);
            OIDSettingsCollection.Add(id, setting);

            return setting;
        }
    }
}
