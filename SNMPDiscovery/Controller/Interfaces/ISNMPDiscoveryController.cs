using System;
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

        //Get current state of state machine
        EnumControllerStates GetCurrentState();
        //Send view possible actions
        EnumControllerStates[] GetStateCommands();
        //Transition on state machine
        bool ChangeState(int stateindex);
        //Define Devices To Discover
        void DefineDevices(string settingID, string initialIP, string finalIP, string SNMPUser);
        //Define Device To Discover
        void EditDevice(string oldSettingID, string settingID, string initialIPAndMask, string finalIPAndMask, string SNMPUser);
        //Delete device and related data
        void DeleteDevice(string settingID);
        //Load Devices Definition
        void LoadDeviceDefinitions();
        //Load Discovered data
        void LoadDiscoveryData();
        //Define processes to execute
        void DefineProcesses(string settingID, EnumProcessingType ProcessType);
        //Edit processes to execute
        void EditProcess(EnumProcessingType previousProcessType, EnumProcessingType ProcessType);
        //Delete process
        void DeleteProcess(EnumProcessingType ProcessType);
        //Start discovery
        void RunDiscovery();
        //Start processes
        void RunProcesses();
        //Pull specific data
        object PullDataList(Type dataType, string key = null);
        //Save Discovered data
        void SaveDiscoveryData();
        //Save Processed data
        void SaveProcessedData();
    }
}
