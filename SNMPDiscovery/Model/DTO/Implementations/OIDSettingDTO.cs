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
        private IList<IObserver<IOIDSettingDTO>> _oidsettingObservers;

        public string ID { get; set; }
        public string InitialOID { get; set; }
        public string FinalOID { get; set; }
        public bool InclusiveInterval { get; set; }
        public IDictionary<string, IList<EnumSNMPOIDIndexType>> IndexedOIDSettings { get; set; }

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

        public IDisposable Subscribe(IObserver<IOIDSettingDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_oidsettingObservers.Contains(observer))
            {
                _oidsettingObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<IOIDSettingDTO>(_oidsettingObservers, observer);
        }

        #endregion

        #region Constructors

        public OIDSettingDTO()
        {
            _oidsettingObservers = new List<IObserver<IOIDSettingDTO>>();
        }

        public OIDSettingDTO(string id, string initialOID, string finalOID, bool inclusiveInterval, IDictionary<string, IList<EnumSNMPOIDIndexType>> indexedOIDSettings = null)
        {
            _oidsettingObservers = new List<IObserver<IOIDSettingDTO>>();

            ID = id;
            InitialOID = initialOID;
            FinalOID = finalOID;
            InclusiveInterval = inclusiveInterval;
            IndexedOIDSettings = indexedOIDSettings == null ? new Dictionary<string, IList<EnumSNMPOIDIndexType>>() : indexedOIDSettings;
        }

        #endregion
    }
}
