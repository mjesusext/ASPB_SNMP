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
        private IList<IObserver<ISNMPRawEntryDTO>> _snmpRawEntryObservers;

        public string OID { get; set; }
        public string RootOID { get; set; }
        public string ValueData { get; set; }
        public EnumSNMPOIDType DataType { get; set; }

        #region Interface Implementations

        public IDisposable Subscribe(IObserver<ISNMPRawEntryDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_snmpRawEntryObservers.Contains(observer))
            {
                _snmpRawEntryObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<ISNMPRawEntryDTO>(_snmpRawEntryObservers, observer);
        }

        #endregion

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
