using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPSettingDTO
    {
        string ID { get; set;}
        IPAddress InitialIP { get; set; }
        IPAddress FinalIP { get; set; }
        string CommunityString { get; set; }
        IDictionary<string, ISNMPProcessingProfileDTO> ProcessingProfiles { get; set;}

        ISNMPProcessingProfileDTO BuildProcessingProfile(string id, EnumProcessingType Process);
    }
}
