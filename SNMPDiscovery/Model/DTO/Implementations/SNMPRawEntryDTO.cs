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
        public event Action<Type, object> OnChange;

        #region Constructors

        public SNMPRawEntryDTO(string oid, Action<Type, object> ChangeTrackerHandler)
        {
            OID = oid;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(typeof(ISNMPDeviceDTO), this);
        }

        public SNMPRawEntryDTO(string oid, string data, EnumSNMPOIDType datatype, Action<Type, object> ChangeTrackerHandler)
        {
            OID = oid;
            ValueData = data;
            DataType = datatype;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(typeof(ISNMPDeviceDTO), this);
        }

        #endregion

    }
}
