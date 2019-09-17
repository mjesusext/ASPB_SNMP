using SNMPDiscovery.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.Helpers
{
    public static class StrategyHelper
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
            if(Device.SNMPRawDataEntries == null)
            {
                return null;
            }

            if (currentRoot != nextRoot)
            {
                return Device.SNMPRawDataEntries.Where(x => CompareOID(x.Key, currentRoot) >= 0 && CompareOID(x.Key, nextRoot) < 0)
                                            .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
                                            .Select(x => x.Value)
                                            .ToList();
            }
            else
            {
                return Device.SNMPRawDataEntries.Where(x => CompareOID(x.Key, currentRoot) >= 0 && CompareOID(x.Key, nextRoot) <= 0)
                                            .OrderBy(x => x.Key, Comparer<string>.Create(CompareOID))
                                            .Select(x => x.Value)
                                            .ToList();
            }
        }

        public static void OIDEntryProcessor(ISNMPDeviceDTO Device, object StrategyDTOobject, IOIDSettingDTO SelectedSetting, IList<Action<IList<string>, string, object>> MappingHandler)
        {
            int numRootEntries = SelectedSetting.IndexedOIDSettings.Count;
            List<string> RootEntries = SelectedSetting.IndexedOIDSettings.Keys.ToList();

            //Loop of each subset 
            for (int i = 0; i < numRootEntries; i++)
            {
                //1) select OID data subset
                IList<ISNMPRawEntryDTO> SelectedDeviceOID = OIDDataSelector(Device, RootEntries[i], i + 1 == numRootEntries ? RootEntries[i] : RootEntries[i + 1]);

                if(SelectedDeviceOID != null)
                {
                    //2) apply specific handle on entryparser
                    OIDEntryParser(SelectedDeviceOID, SelectedSetting.IndexedOIDSettings[RootEntries[i]], StrategyDTOobject, MappingHandler[i]);
                }
            }
        }

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
                        IndexData.Add(string.Join(" ", indexValues.Take(6).Select(x => x.ToString("X").PadLeft(2,'0'))));
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

        public static string[] GetFlagArrayPositions(string mask)
        {
            string[] result;

            if (mask.Contains(","))
            {
                result = mask.Split(new string[1] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                IEnumerable<string> BinaryMaskComponents = mask.Replace(" ", "").Select(x => Convert.ToString(int.Parse(x.ToString(), System.Globalization.NumberStyles.HexNumber), 2).PadLeft(4, '0'));
                string BinaryMask = string.Concat(BinaryMaskComponents);

                result = new string[BinaryMask.Count(x => x == '1')];
                int indresult = 0;
                
                for (int i = 0; i < BinaryMask.Length; i++)
                {
                    if(BinaryMask[i] == '1')
                    {
                        result[indresult++] = (i + 1).ToString();
                    }
                }
            }

            return result;
        }
    }
}
