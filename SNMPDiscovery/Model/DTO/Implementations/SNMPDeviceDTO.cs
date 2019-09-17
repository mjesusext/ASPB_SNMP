using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPDeviceDTO : ISNMPDeviceDTO
    {
        private IList<IObserver<ISNMPDeviceDTO>> _snmpDeviceObservers;

        public IPAddress TargetIP { get; set; }
        public IDictionary<string, ISNMPRawEntryDTO> SNMPRawDataEntries { get; set; }
        public IDictionary<string, ISNMPProcessedValueDTO> SNMPProcessedData { get; set; }

        #region Interface Implementations

        public ISNMPRawEntryDTO BuildSNMPRawEntry(string OID)
        {
            //Lazy initialization
            if (SNMPRawDataEntries == null)
            {
                SNMPRawDataEntries = new Dictionary<string, ISNMPRawEntryDTO>();
            }

            ISNMPRawEntryDTO RawEntry = new SNMPRawEntryDTO(OID);
            SNMPRawDataEntries.Add(OID, RawEntry);

            return RawEntry;
        }

        public ISNMPRawEntryDTO BuildSNMPRawEntry(string OID, string RawValue, EnumSNMPOIDType DataType)
        {
            //Lazy initialization
            if (SNMPRawDataEntries == null)
            {
                SNMPRawDataEntries = new Dictionary<string, ISNMPRawEntryDTO>();
            }

            ISNMPRawEntryDTO RawEntry = new SNMPRawEntryDTO(OID, RawValue, DataType);
            SNMPRawDataEntries.Add(OID, RawEntry);

            return RawEntry;
        }

        public IDisposable Subscribe(IObserver<ISNMPDeviceDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_snmpDeviceObservers.Contains(observer))
            {
                _snmpDeviceObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<ISNMPDeviceDTO>(_snmpDeviceObservers, observer);
        }

        #endregion

        #region Helpful methods

        public void AttachSNMPProcessedValue(Type DataType, object Data)
        {
            //Lazy initialization
            if (SNMPProcessedData == null)
            {
                SNMPProcessedData = new Dictionary<string, ISNMPProcessedValueDTO>();
            }

            ISNMPProcessedValueDTO ProcessedValue = new SNMPProcessedValueDTO(DataType, Data);
            SNMPProcessedData.Add(DataType.Name, ProcessedValue);
        }

        #endregion

        #region Constructors

        public SNMPDeviceDTO()
        {
            _snmpDeviceObservers = new List<IObserver<ISNMPDeviceDTO>>();
        }

        public SNMPDeviceDTO(string targetIP)
        {
            _snmpDeviceObservers = new List<IObserver<ISNMPDeviceDTO>>();
            TargetIP = IPAddress.Parse(targetIP);
        }

        public SNMPDeviceDTO(int targetIP)
        {
            _snmpDeviceObservers = new List<IObserver<ISNMPDeviceDTO>>();
            TargetIP = new IPAddress(targetIP);
        }

        #endregion


    }
}
