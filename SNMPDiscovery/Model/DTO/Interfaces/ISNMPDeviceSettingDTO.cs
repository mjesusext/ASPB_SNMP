using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPDeviceSettingDTO : ITrackeableObject
    {
        string ID { get; set;}
        ISNMPModelDTO RegardingObject { get; set; }

        IPAddress InitialIP { get; set; }
        IPAddress FinalIP { get; set; }
        int NetworkMask { get; set; }
        string CommunityString { get; set; }

        void EditDeviceSetting(string newid, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
    }
}
