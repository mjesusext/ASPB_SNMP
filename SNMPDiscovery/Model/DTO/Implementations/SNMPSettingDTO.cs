using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Helpers;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPSettingDTO : ISNMPSettingDTO
    {
        public string ID { get; set; }
        public IPAddress InitialIP { get; set; }
        public IPAddress FinalIP { get; set; }
        public int NetworkMask { get; set; }
        public string CommunityString { get; set; }
        public IDictionary<string, ISNMPProcessStrategy> Processes { get; set; }
        public IDictionary<string, IOIDSettingDTO> OIDSettings { get; set; }

        public event Action<object, Type> OnChange;

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

        public void ChangeTrackerHandler(object obj, Type type)
        {
            OnChange?.Invoke(obj, type);
        }

        #endregion

        #region Constructors

        public SNMPSettingDTO(string id, string initialIPAndMask, string finalIPAndMask, string SNMPUser, Action<object, Type> ChangeTrackerHandler)
        {
            ID = id;
            InitialIP = ModelHelper.ExtractIPAddress(initialIPAndMask);
            FinalIP = finalIPAndMask == null ? InitialIP : ModelHelper.ExtractIPAddress(finalIPAndMask);
            NetworkMask = ModelHelper.ExtractNetworkMask(initialIPAndMask);
            CommunityString = SNMPUser;

            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPSettingDTO));
        }

        #endregion
    }
}