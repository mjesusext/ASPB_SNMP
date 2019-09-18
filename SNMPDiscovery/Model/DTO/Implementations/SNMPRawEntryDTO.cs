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
        public string OID { get; set; }
        public string RootOID { get; set; }
        public string ValueData { get; set; }
        public EnumSNMPOIDType DataType { get; set; }
        public event Action<string> OnChange;

        #region Constructors

        public SNMPRawEntryDTO()
        {
        }

        public SNMPRawEntryDTO(string oid)
        {
            OID = oid;
        }

        public SNMPRawEntryDTO(string oid, string data, EnumSNMPOIDType datatype)
        {
            OID = oid;
            ValueData = data;
            DataType = datatype;
        }

        #endregion

    }
}
