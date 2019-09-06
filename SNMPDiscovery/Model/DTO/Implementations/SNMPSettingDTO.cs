using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPSettingDTO : ISNMPSettingDTO
    {
        public string ID { get; set; }
        public IPAddress InitialIP { get; set; }
        public IPAddress FinalIP { get; set; }
        public string CommunityString { get; set; }
        public IDictionary<string, ISNMPProcessingProfileDTO> ProcessingProfiles { get; set; }

        public SNMPSettingDTO()
        {
        }

        public SNMPSettingDTO(string id, string initialIP, string finalIP, string SNMPUser)
        {
            ID = id;
            InitialIP = IPAddress.Parse(initialIP);
            FinalIP = finalIP == null ? InitialIP : IPAddress.Parse(finalIP);
            CommunityString = SNMPUser;
        }

        public ISNMPProcessingProfileDTO BuildProcessingProfile(string id, EnumProcessingType Process)
        {
            //Lazy initialization
            if (ProcessingProfiles == null)
            {
                ProcessingProfiles = new Dictionary<string, ISNMPProcessingProfileDTO>();
            }

            ISNMPProcessingProfileDTO ProcessProfile = new SNMPProcessingProfile(id, Process);
            ProcessingProfiles.Add(id, ProcessProfile);

            return ProcessProfile;
        }

    }
}
