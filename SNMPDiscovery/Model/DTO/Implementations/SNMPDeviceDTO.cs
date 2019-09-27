using SNMPDiscovery.Model.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPDeviceDTO : ISNMPDeviceDTO
    {
        public IPAddress TargetIP { get; set; }
        public int NetworkMask { get; set; }
        public IDictionary<string, ISNMPRawEntryDTO> SNMPRawDataEntries { get; set; }
        public IDictionary<string, ISNMPProcessedValueDTO> SNMPProcessedData { get; set; }

        public event Action<object, Type> OnChange;

        #region Interface Implementations

        public ISNMPRawEntryDTO BuildSNMPRawEntry(string OID)
        {
            //Lazy initialization
            if (SNMPRawDataEntries == null)
            {
                SNMPRawDataEntries = new Dictionary<string, ISNMPRawEntryDTO>();
            }

            ISNMPRawEntryDTO RawEntry = new SNMPRawEntryDTO(OID, OnChange);
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

            ISNMPRawEntryDTO RawEntry = new SNMPRawEntryDTO(OID, RawValue, DataType, OnChange);
            SNMPRawDataEntries.Add(OID, RawEntry);

            return RawEntry;
        }

        #endregion

        #region Nested Object Change Handlers

        public void ChangeTrackerHandler(Type type, object obj)
        {
            OnChange?.Invoke(obj, type);
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

            ISNMPProcessedValueDTO ProcessedValue = new SNMPProcessedValueDTO(DataType, Data, OnChange);
            SNMPProcessedData.Add(DataType.Name, ProcessedValue);
        }

        #endregion

        #region Constructors

        public SNMPDeviceDTO(IPAddress targetIP, int networkMask, Action<object, Type> ChangeTrackerHandler)
        {
            TargetIP = targetIP;
            NetworkMask = networkMask;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPDeviceDTO));
        }

        public SNMPDeviceDTO(string targetIP, int networkMask, Action<object, Type> ChangeTrackerHandler)
        {
            TargetIP = IPAddress.Parse(targetIP);
            NetworkMask = networkMask;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPDeviceDTO));
        }

        public SNMPDeviceDTO(int targetIP, int networkMask, Action<object, Type> ChangeTrackerHandler)
        {
            TargetIP = new IPAddress(targetIP);
            NetworkMask = networkMask;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPDeviceDTO));
        }

        #endregion
    }
}
