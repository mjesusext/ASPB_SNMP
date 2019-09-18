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
        public IDictionary<string, IList<EnumSNMPOIDIndexType>> IndexedOIDSettings { get; set; }
        public event Action<object, Type> OnChange;

        #region Interface implementations

        public IDictionary<string, IList<EnumSNMPOIDIndexType>> BuildIndexedOIDSetting(string rootOID, IList<EnumSNMPOIDIndexType> indexDataDefs)
        {
            //Lazy initialization
            if (IndexedOIDSettings == null)
            {
                IndexedOIDSettings = new Dictionary<string, IList<EnumSNMPOIDIndexType>>();
            }

            IndexedOIDSettings.Add(rootOID, indexDataDefs);

            return IndexedOIDSettings;
        }

        #endregion

        #region Constructors

        public OIDSettingDTO(string id, string initialOID, string finalOID, bool inclusiveInterval, IDictionary<string, IList<EnumSNMPOIDIndexType>> indexedOIDSettings, Action<object, Type> ChangeTrackerHandler)
        {
            ID = id;
            InitialOID = initialOID;
            FinalOID = finalOID;
            InclusiveInterval = inclusiveInterval;
            IndexedOIDSettings = indexedOIDSettings ?? new Dictionary<string, IList<EnumSNMPOIDIndexType>>();
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(IOIDSettingDTO));
        }

        #endregion
    }
}
