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
        public IPAddress TargetIP { get; set; }
        public IDictionary<string, ISNMPRawEntryDTO> SNMPRawDataEntries { get; set; }
        public IDictionary<string, ISNMPProcessedValueDTO> SNMPProcessedData { get; set; }

        public SNMPDeviceDTO()
        {
        }

        public SNMPDeviceDTO(string targetIP)
        {
            TargetIP = IPAddress.Parse(targetIP);
        }

        public SNMPDeviceDTO(int targetIP)
        {
            TargetIP = new IPAddress(targetIP);
        }

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
    }
}
