using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPDeviceDTO : ITrackeableObject
    {
        IPAddress TargetIP { get; set; }
        int NetworkMask { get; set; }
        IDictionary<string, ISNMPRawEntryDTO> SNMPRawDataEntries { get; set; }
        IDictionary<string, ISNMPProcessedValueDTO> SNMPProcessedData { get; set; }

        ISNMPRawEntryDTO BuildSNMPRawEntry(string OID);
        ISNMPRawEntryDTO BuildSNMPRawEntry(string OID, string RawValue, EnumSNMPOIDType DataType);
        ISNMPProcessedValueDTO AttachSNMPProcessedValue(Type DataType, object Data);
    }
}
