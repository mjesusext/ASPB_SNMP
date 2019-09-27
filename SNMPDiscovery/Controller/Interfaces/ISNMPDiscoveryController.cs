﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Controller
{
    public interface ISNMPDiscoveryController
    {
        IObserver<ISNMPModelDTO> View { get; set; }
        ISNMPModelDTO Model { get; set; }
        event Action<List<string>> OnInvalidInputs;
        
        //Define Devices To Discover
        void DefineDevice(string settingID, string initialIP, string finalIP, string SNMPUser);
        //Load Discovered data
        void LoadDiscoveryData();
        //Define processes to execute
        void DefineProcesses(string settingID, EnumProcessingType processType);
        //Start discovery
        void RunDiscovery();
        //Pull specific data
        object PullData(Type dataType, string key = null);
        //Save Discovered data
        void SaveDiscoveryData();
        //Save Processed data
        void SaveProcessedData();
    }
}
