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

        public static IList<ISNMPRawEntryDTO> OIDDataSelector(ISNMPDeviceDTO Device, string currentRoot, string nextRoot)
        {
            return Device.SNMPRawDataEntries.Where(x => CompareOID(x.Key, currentRoot) >= 0 && CompareOID(x.Key, nextRoot) <= 0)
                                            .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
                                            .Select(x => x.Value)
                                            .ToList();
        }

        public static void OIDEntryProcessor() { }

        public static void OIDEntryParser(IList<ISNMPRawEntryDTO> SelectedDeviceOID, IIndexedOIDSettingDTO IndexOIDSetting, object StrategyDTOobject, Action<IList<string>, string, object> MappingHandler)
        {
            IList<string> IndexData = new List<string>();

            //Iterate on selected data for parsing possible indexes --> Not apliying here
            foreach (ISNMPRawEntryDTO RawEntry in SelectedDeviceOID)
            {
                OIDIndexEntryParser(IndexOIDSetting, RawEntry, IndexData);

                //Save results and clean decoded index list
                if(MappingHandler != null)
                {
                    MappingHandler(IndexData, RawEntry.ValueData, StrategyDTOobject);
                }
                
                IndexData.Clear();
            }
        }

        public static void OIDIndexEntryParser(IIndexedOIDSettingDTO IndexOIDSetting, ISNMPRawEntryDTO RawEntry, IList<string> IndexData)
        {
            List<int> indexValues = RawEntry.OID.Replace(IndexOIDSetting.RootOID + ".", "").Split('.').Select(x => int.Parse(x)).ToList();

            foreach (EnumSNMPOIDIndexType IndexType in IndexOIDSetting.IndexDataDefinitions)
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
