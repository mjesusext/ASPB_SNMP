using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class OIDSettingDTO : IOIDSettingDTO
    {
        public string ID { get; set; }
        public string InitialOID { get; set; }
        public string FinalOID { get; set; }
        public bool InclusiveInterval { get; set; }
        public IDictionary<string, IIndexedOIDSettingDTO> IndexedOIDSettings { get; set; }

        public OIDSettingDTO()
        {
        }

        public OIDSettingDTO(string id, string initialOID, string finalOID, bool inclusiveInterval, IDictionary<string, IIndexedOIDSettingDTO> indexedOIDSettings = null)
        {
            ID = id;
            InitialOID = initialOID;
            FinalOID = finalOID;
            InclusiveInterval = inclusiveInterval;
            IndexedOIDSettings = indexedOIDSettings == null ? new Dictionary<string,IIndexedOIDSettingDTO>() : indexedOIDSettings;
        }

        public IIndexedOIDSettingDTO BuildIndexedOIDSetting(string rootOID, IList<EnumSNMPOIDIndexType> indexDataDefs)
        {
            //Lazy initialization
            if (IndexedOIDSettings == null)
            {
                IndexedOIDSettings = new Dictionary<string, IIndexedOIDSettingDTO>();
            }

            IIndexedOIDSettingDTO setting = new IndexedOIDSettingDTO(rootOID, indexDataDefs);
            IndexedOIDSettings.Add(rootOID, setting);

            return setting;
        }
    }
}
