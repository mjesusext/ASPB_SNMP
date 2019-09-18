using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPSettingDTO : ISNMPSettingDTO
    {
        public string ID { get; set; }
        public IPAddress InitialIP { get; set; }
        public IPAddress FinalIP { get; set; }
        public string CommunityString { get; set; }
        public IDictionary<string, ISNMPProcessStrategy> Processes { get; set; }
        public IDictionary<string, IOIDSettingDTO> OIDSettings { get; set; }

        public IDictionary<Type, IList> ChangedObjects { get; set; }
        public event Action<Type, object> OnChange;

        #region Interface implementations

        public ISNMPProcessStrategy BuildProcess(EnumProcessingType ProcessType)
        {
            ISNMPProcessStrategy ProcessProfile = null;

            //Lazy initialization
            if (Processes == null)
            {
                Processes = new Dictionary<string, ISNMPProcessStrategy>();
            }

            switch (ProcessType)
            {
                case EnumProcessingType.None:
                    break;
                case EnumProcessingType.TopologyDiscovery:
                    ProcessProfile = new TopologyBuilderStrategy(ChangeTrackerHandler);
                    break;
                case EnumProcessingType.PrinterConsumption:
                    break;
                default:
                    break;
            }
            
            //If profile exists, retrive the existing one
            if (Processes.ContainsKey(ProcessProfile.ProcessID))
            {
                return Processes[ProcessProfile.ProcessID];
            }
            else
            {
                OIDSettings = ProcessProfile.BuildOIDSetting(ID, OIDSettings);
                Processes.Add(ProcessProfile.ProcessID, ProcessProfile);

                return ProcessProfile;
            }
        }

        #endregion

        #region Nested Object Change Handlers

        public void ChangeTrackerHandler(Type type, object obj)
        {
            ChangedObjects[type].Add(obj);
        }

        #endregion

        #region Constructors

        public SNMPSettingDTO(string id, string initialIP, string finalIP, string SNMPUser, Action<Type, object> ChangeTrackerHandler)
        {
            ID = id;
            InitialIP = IPAddress.Parse(initialIP);
            FinalIP = finalIP == null ? InitialIP : IPAddress.Parse(finalIP);
            CommunityString = SNMPUser;

            ChangedObjects = new Dictionary<Type, IList>();
            ChangedObjects.Add(typeof(ISNMPProcessStrategy), new ArrayList());
            ChangedObjects.Add(typeof(IOIDSettingDTO), new ArrayList());
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(typeof(ISNMPSettingDTO), this);
        }

        #endregion
    }
}
