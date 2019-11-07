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
    public class SNMPDeviceDataDTO : ISNMPDeviceDataDTO
    {
        public ISNMPModelDTO RegardingObject { get; set; }
        public IPAddress TargetIP { get; set; }
        public string MACAddress { get; set; }
        public int NetworkMask { get; set; }
        public IDictionary<string, ISNMPRawEntryDTO> SNMPRawDataEntries { get; set; }
        public IDictionary<string, ISNMPProcessedValueDTO> SNMPProcessedData { get; set; }
        
        public event Action<object, Type> OnChange;

        #region Interface Implementations

        public ISNMPRawEntryDTO BuildSNMPRawEntry(string OID, string RawValue, EnumSNMPOIDType DataType)
        {
            //Lazy initialization
            if (SNMPRawDataEntries == null)
            {
                SNMPRawDataEntries = new Dictionary<string, ISNMPRawEntryDTO>();
            }

            ISNMPRawEntryDTO RawEntry = new SNMPRawEntryDTO(this, OID, RawValue, DataType);
            SNMPRawDataEntries.Add(OID, RawEntry);

            //We know data is fully ready
            OnChange?.Invoke(RawEntry, typeof(ISNMPRawEntryDTO));

            return RawEntry;
        }

        public ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data)
        {
            //Lazy initialization
            if (SNMPProcessedData == null)
            {
                SNMPProcessedData = new Dictionary<string, ISNMPProcessedValueDTO>();
            }

            ISNMPProcessedValueDTO ProcessedValue = new SNMPProcessedValueDTO(DataType, Data);
            SNMPProcessedData.Add(DataType.Name, ProcessedValue);
            //We don't trigger OnChange because we only set the poiner, information is still not filled.

            return ProcessedValue;
        }

        #endregion

        #region Constructors

        public SNMPDeviceDataDTO(IPAddress targetIP, int networkMask, Action<object, Type> ChangeTrackerHandler)
        {
            TargetIP = targetIP;
            NetworkMask = networkMask;
            OnChange += ChangeTrackerHandler;

            //We know data is fully ready
            OnChange?.Invoke(this, typeof(ISNMPDeviceDataDTO));
        }

        #endregion
    }
}
