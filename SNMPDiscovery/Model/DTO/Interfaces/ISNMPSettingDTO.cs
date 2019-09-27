﻿using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPSettingDTO : ITrackeableObject
    {
        string ID { get; set;}
        IPAddress InitialIP { get; set; }
        IPAddress FinalIP { get; set; }
        int NetworkMask { get; set; }
        string CommunityString { get; set; }
        IDictionary<string, ISNMPProcessStrategy> Processes { get; set; }
        IDictionary<string, IOIDSettingDTO> OIDSettings { get; set; }
        
        ISNMPProcessStrategy BuildProcess(EnumProcessingType Process);
    }
}
