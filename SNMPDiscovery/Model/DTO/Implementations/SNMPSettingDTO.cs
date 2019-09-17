﻿using System;
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
        private IList<IObserver<ISNMPSettingDTO>> _snmpSettingObservers;

        public string ID { get; set; }
        public IPAddress InitialIP { get; set; }
        public IPAddress FinalIP { get; set; }
        public string CommunityString { get; set; }
        public IDictionary<string, ISNMPProcessStrategy> Processes { get; set; }
        public IDictionary<string, IOIDSettingDTO> OIDSettings { get; set; }

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
                    ProcessProfile = new TopologyBuilderStrategy();
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

        public IDisposable Subscribe(IObserver<ISNMPSettingDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_snmpSettingObservers.Contains(observer))
            {
                _snmpSettingObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<ISNMPSettingDTO>(_snmpSettingObservers, observer);
        }

        #endregion

        #region Constructors

        public SNMPSettingDTO()
        {
        }

        public SNMPSettingDTO(string id, string initialIP, string finalIP, string SNMPUser)
        {
            ID = id;
            InitialIP = IPAddress.Parse(initialIP);
            FinalIP = finalIP == null ? InitialIP : IPAddress.Parse(finalIP);
            CommunityString = SNMPUser;
        }

        #endregion
    }
}
