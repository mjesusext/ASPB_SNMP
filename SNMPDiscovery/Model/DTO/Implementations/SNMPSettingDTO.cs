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
        public string ID { get; set; }
        public IPAddress InitialIP { get; set; }
        public IPAddress FinalIP { get; set; }
        public string CommunityString { get; set; }
        public IDictionary<string, ISNMPProcessStrategy> Processes { get; set; }
        public IDictionary<string, IOIDSettingDTO> OIDSettings { get; set; }

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

        public ISNMPProcessStrategy BuildProcess(string id, EnumProcessingType ProcessType)
        {
            ISNMPProcessStrategy ProcessProfile = null;

            //Lazy initialization
            if (Processes == null)
            {
                Processes = new Dictionary<string, ISNMPProcessStrategy>();
            }

            //If profile exists, retrive the existing one
            if (Processes.ContainsKey(id))
            {
                return Processes[id];
            }
            else
            {
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

                OIDSettings = ProcessProfile.BuildOIDSetting(OIDSettings);
                Processes.Add(id, ProcessProfile);

                return ProcessProfile;
            }
        }
    }
}
