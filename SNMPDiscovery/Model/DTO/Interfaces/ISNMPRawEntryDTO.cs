using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    //Leafs of DTO object hierarchy don't need onchange events
    public interface ISNMPRawEntryDTO
    {
        ISNMPDeviceDataDTO RegardingObject { get; set; }
        string OID { get; set; }
        string ValueData { get; set; }
        EnumSNMPOIDType DataType { get; set; }
    }
}
