using SNMPDiscovery.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.Helpers
{
    public static class ModelHelper
    {
        public static int CompareOID(string current, string reference)
        {
            //Convert to integers for comparison
            int[] currOID = current.Split('.').Select(x => int.Parse(x)).ToArray();
            int[] refOID = reference.Split('.').Select(x => int.Parse(x)).ToArray();
        
            int maxindex = refOID.Length.CompareTo(currOID.Length) > 0 ? currOID.Length : refOID.Length;

            for (int i = 0; i < maxindex; i++)
            {
                if (refOID[i] < currOID[i])
                {
                    return 1;
                }
                else if (refOID[i] > currOID[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        public static IList<ISNMPRawEntryDTO> OIDDataSelector(ISNMPDeviceDTO Device, IOIDSettingDTO SelectedSetting)
        {
            return Device.SNMPRawDataEntries.Where(x => CompareOID(x.Key, SelectedSetting.InitialOID) >= 0 &&
                                                (
                                                    CompareOID(x.Key, SelectedSetting.FinalOID) <= 0 && SelectedSetting.InclusiveInterval ||
                                                    CompareOID(x.Key, SelectedSetting.FinalOID) < 0 && !SelectedSetting.InclusiveInterval)
                                                )
                                            .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
                                            .Select(x => x.Value)
                                            .ToList();
        }

        public static bool HasOIDIndexInfo(IOIDSettingDTO SelectedSetting, ISNMPRawEntryDTO RawEntry, out string RootOID)
        {
            RootOID = SelectedSetting.IndexedOIDSettings.Keys.Where(x => RawEntry.OID.StartsWith(x)).FirstOrDefault();

            return RootOID != null;
        }

        public static void OIDEntryParser(ISNMPDeviceDTO Device, IOIDSettingDTO SelectedSetting, object StrategyDTOobject, Action<IList<string>, string, object> MappingHandler)
        {
            string rootOID;
            IList<ISNMPRawEntryDTO> SelectedData = OIDDataSelector(Device, SelectedSetting);
            IList<string> IndexData = new List<string>();

            //Iterate on selected data for parsing possible indexes --> Not apliying here
            foreach (ISNMPRawEntryDTO RawEntry in SelectedData)
            {
                if(HasOIDIndexInfo(SelectedSetting, RawEntry, out rootOID))
                {
                    OIDIndexEntryParser(SelectedSetting, RawEntry, rootOID, IndexData);
                }
                else
                {
                    continue;
                }
                
                //Save results and clean decoded index list
                MappingHandler(IndexData, RawEntry.ValueData, StrategyDTOobject);
                IndexData.Clear();
            }
        }

        public static void OIDIndexEntryParser(IOIDSettingDTO SelectedSetting, ISNMPRawEntryDTO RawEntry, string RootOID, IList<string> IndexData)
        {
            List<int> indexValues = RawEntry.OID.Replace(RootOID + ".", "").Split('.').Select(x => int.Parse(x)).ToList();

            foreach (EnumSNMPOIDIndexType IndexType in SelectedSetting.IndexedOIDSettings[RootOID].IndexDataDefinitions)
            {
                switch (IndexType)
                {
                    case EnumSNMPOIDIndexType.Number:
                        IndexData.Add(indexValues[0].ToString());
                        indexValues.RemoveAt(0);

                        break;
                    case EnumSNMPOIDIndexType.MacAddress:
                        IndexData.Add(string.Join(" ", indexValues.Take(6).Select(x => x.ToString("X"))));
                        indexValues.RemoveRange(0, 6);

                        break;
                    case EnumSNMPOIDIndexType.IP:
                        break;
                    case EnumSNMPOIDIndexType.Date:
                        break;
                    case EnumSNMPOIDIndexType.ByteString:
                        break;
                    case EnumSNMPOIDIndexType.Oid:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
