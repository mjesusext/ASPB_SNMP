using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPRawEntryDTO : ISNMPRawEntryDTO
    {
        public ISNMPDeviceDataDTO RegardingObject { get; set; }
        public string OID { get; set; }
        public string RootOID { get; set; }
        public string ValueData { get; set; }
        public EnumSNMPOIDType DataType { get; set; }

        #region Constructors

        public SNMPRawEntryDTO(string oid, string data, EnumSNMPOIDType datatype)
        {
            OID = oid;
            ValueData = data;
            DataType = datatype;
        }

        #endregion

    }
}
