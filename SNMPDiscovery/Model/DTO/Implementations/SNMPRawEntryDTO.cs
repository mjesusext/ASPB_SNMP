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
        public event Action<object, Type> OnChange;

        #region Constructors

        public SNMPRawEntryDTO(string oid, Action<object, Type> ChangeTrackerHandler)
        {
            OID = oid;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPDeviceDTO));
        }

        public SNMPRawEntryDTO(string oid, string data, EnumSNMPOIDType datatype, Action<object, Type> ChangeTrackerHandler)
        {
            OID = oid;
            ValueData = data;
            DataType = datatype;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPRawEntryDTO));
        }

        #endregion

    }
}
