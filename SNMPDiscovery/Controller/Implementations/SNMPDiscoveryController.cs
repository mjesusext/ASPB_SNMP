﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;
using SNMPDiscovery.View;
using SNMPDiscovery.Model.Helpers;

namespace SNMPDiscovery.Controller
{
    public class SNMPDiscoveryController : ISNMPDiscoveryController
    {
        #region Public fields

        public event Action<List<string>> OnInvalidInputs;
        
        #endregion

        #region Private properties

        private IObserver<ISNMPModelDTO> View { get; set; }
        private ISNMPModelDTO Model { get; set; }

        private List<string> _ErrMsgs { get; set; }
        private Dictionary<EnumControllerStates, EnumControllerStates[]> _StateMachine { get; set; }
        private Stack<EnumControllerStates> _StateHistory { get; set; }

        #endregion

        #region Controller Implementation

        public EnumControllerStates GetCurrentState()
        {
            return _StateHistory.Peek();
        }

        public EnumControllerStates[] GetStateCommands()
        {
            return _StateMachine[_StateHistory.Peek()];
        }

        public bool ChangeState(int stateindex)
        {
            bool res = false;

            if (stateindex >= _StateMachine[_StateHistory.Peek()].Length)
            {
                return res;
            }
            else
            {
                res = true;
            }

            if (_StateMachine[_StateHistory.Peek()][stateindex] == EnumControllerStates.BackAction)
            {
                _StateHistory.Pop();
            }
            else
            {
                _StateHistory.Push(_StateMachine[_StateHistory.Peek()][stateindex]);
            }

            return res;
        }

        public void DefineDevices(string settingID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            bool addrformatKO = false;

            if (string.IsNullOrWhiteSpace(settingID))
            {
                addrformatKO = true;
                _ErrMsgs.Add("Null or empty setting ID");
            }

            if (!ModelHelper.ValidateIPAndMask(initialIPAndMask))
            {
                addrformatKO = true;
                _ErrMsgs.Add("Invalid initial IP");
            }

            if (!ModelHelper.ValidateIPAndMask(finalIPAndMask))
            {
                addrformatKO = true;
                _ErrMsgs.Add("Invalid final IP");
            }

            if(!addrformatKO && !ModelHelper.ValidateIPandMaskRange(initialIPAndMask, finalIPAndMask))
            {
                _ErrMsgs.Add("Invalid IP range");
            }

            if (_ErrMsgs.Count == 0)
            {
                //Consume data
                Model.BuildSNMPSetting(settingID, initialIPAndMask, finalIPAndMask, SNMPUser);
            }
            else
            {
                NotifyControllerError();
            }
        }

        public void EditDevice(string oldSettingID, string settingID, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            if (string.IsNullOrWhiteSpace(oldSettingID))
            {
                _ErrMsgs.Add("Null or empty old setting ID");
            }

            if (string.IsNullOrWhiteSpace(settingID))
            {
                _ErrMsgs.Add("Null or empty setting ID");
            }

            if (!ModelHelper.ValidateIPAndMask(initialIPAndMask))
            {
                _ErrMsgs.Add("Invalid initial IP");
            }

            if (!ModelHelper.ValidateIPAndMask(finalIPAndMask))
            {
                _ErrMsgs.Add("Invalid final IP");
            }

            if(!ModelHelper.ValidateIPandMaskRange(initialIPAndMask, finalIPAndMask))
            {
                _ErrMsgs.Add("Invalid IP range");
            }

            if (string.IsNullOrWhiteSpace(SNMPUser))
            {
                _ErrMsgs.Add("Null or empty SNMP community string");
            }

            if (_ErrMsgs.Count == 0)
            {
                //Consume data
                Model.EditSNMPSetting(oldSettingID, settingID, initialIPAndMask, finalIPAndMask, SNMPUser);
            }
            else
            {
                NotifyControllerError();
            }
        }

        public void DeleteDevice(string settingID)
        {
            Model.DeleteSNMPSetting(settingID);
        }

        public void LoadDiscoveryData() { }

        public void LoadDeviceDefinitions() { }

        public void DefineProcesses(string settingID, EnumProcessingType processType)
        {
            Model.BuildProcess(settingID, processType);
        }

        public void EditProcess(EnumProcessingType previousProcessType, EnumProcessingType processType)
        {
            Model.EditProcess(previousProcessType, processType);
        }

        public void DeleteProcess(EnumProcessingType ProcessType)
        {
            Model.DeleteProcess(ProcessType);
        }

        public void RunDiscovery()
        {
            Model.StartDiscovery();
        }

        public void RunProcesses()
        {
            if (Model.Processes?.Values == null)
            {
                _ErrMsgs.Add("No processes are defined for analysis");
                NotifyControllerError();
                return;
            }

            foreach (ISNMPProcessStrategy proccess in Model.Processes.Values)
            {
                proccess.Run();
            }
        }

        public IList<ISNMPDeviceDataDTO> GetSNMPDeviceData(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.DeviceData.Values.ToList();
            }
            else
            {
                ISNMPDeviceDataDTO res = null;
                Model.DeviceData.TryGetValue(key, out res);

                return res == null ? new List<ISNMPDeviceDataDTO>() : new List<ISNMPDeviceDataDTO>(new[] { res });
            }
        }

        public IList<ISNMPDeviceSettingDTO> GetSNMPDeviceSetting(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.DeviceSettings.Values.ToList();
            }
            else
            {
                ISNMPDeviceSettingDTO res = null;
                Model.DeviceSettings.TryGetValue(key, out res);

                return res == null ? new List<ISNMPDeviceSettingDTO>() : new List<ISNMPDeviceSettingDTO>(new[] { res });
            }
        }

        public IList<ISNMPProcessStrategy> GetSNMPProcessStrategy(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.Processes.Values.ToList();
            }
            else
            {
                ISNMPProcessStrategy res = null;

                EnumProcessingType skey = EnumProcessingType.None;
                if (Enum.TryParse<EnumProcessingType>(key, out skey))
                {
                    Model.Processes.TryGetValue(skey, out res);
                }

                return res == null ? new List<ISNMPProcessStrategy>() : new List<ISNMPProcessStrategy>(new[] { res });
            }
        }

        public IList<IOIDSettingDTO> GetOIDSetting(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.Processes.Values.SelectMany(x => x.OIDSettings.Values).ToList();
            }
            else
            {
                IEnumerable<IOIDSettingDTO> res = Model.Processes.Values.Select(x => x.OIDSettings.ContainsKey(key) ? x.OIDSettings[key] : null);

                return res.Count() == 0 ? new List<IOIDSettingDTO>() : new List<IOIDSettingDTO>(res);
            }
        }

        public IList<ISNMPRawEntryDTO> GetSNMPRawEntry(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.DeviceData.Values.SelectMany(x => x.SNMPRawDataEntries.Values).ToList();
            }
            else
            {
                IEnumerable<ISNMPRawEntryDTO> res = Model.DeviceData.Values.Select(x => x.SNMPRawDataEntries.ContainsKey(key) ? x.SNMPRawDataEntries[key] : null);

                return res.Count() == 0 ? new List<ISNMPRawEntryDTO>() : new List<ISNMPRawEntryDTO>(res);
            }
        }

        public IList<ISNMPProcessedValueDTO> GetSNMPDeviceProcessedValue(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.DeviceData.Values.SelectMany(x => x.SNMPProcessedData.Values).ToList();
            }
            else
            {
                IEnumerable<ISNMPProcessedValueDTO> res = Model.DeviceData.Values.Select(x => x.SNMPProcessedData.ContainsKey(key) ? x.SNMPProcessedData[key] : null);

                return res.Count() == 0 ? new List<ISNMPProcessedValueDTO>() : new List<ISNMPProcessedValueDTO>(res);
            }
        }

        public IList<ISNMPProcessedValueDTO> GetSNMPGlobalProcessedValue(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Model.GlobalProcessedData.Values.ToList();
            }
            else
            {
                ISNMPProcessedValueDTO res = null;

                return Model.GlobalProcessedData.TryGetValue(key, out res) ? new List<ISNMPProcessedValueDTO>() : new List<ISNMPProcessedValueDTO>(new[] { res });
            }
        }

        public void SaveDiscoveryData() { }

        public void SaveProcessedData() { }

        #endregion

        #region Private methods

        private void NotifyControllerError()
        {
            OnInvalidInputs?.Invoke(_ErrMsgs);
            _ErrMsgs.Clear();
            _StateHistory.Pop();
        }

        #endregion

        #region Initializers - Finalizers

        public SNMPDiscoveryController(ISNMPModelDTO ModelService)
        {
            InitController();
            Model = ModelService;
            View = new SNMPDiscoveryView(ModelService, this);
        }

        private void InitController()
        {
            _ErrMsgs = new List<string>();
            _StateHistory = new Stack<EnumControllerStates>(new EnumControllerStates[] { EnumControllerStates.Main });

            EnumControllerStates[] CommonDefsOptionSet = new EnumControllerStates[] { EnumControllerStates.AddDeviceDefinition, EnumControllerStates.ShowDeviceDefinitions, EnumControllerStates.EditDeviceDefinition, EnumControllerStates.DeleteDeviceDefinition, EnumControllerStates.AddProcessDefinition, EnumControllerStates.ShowProcessDefinitions, EnumControllerStates.EditProcessDefinition, EnumControllerStates.DeleteProcessDefinition, EnumControllerStates.RunProcess, EnumControllerStates.BackAction };
            EnumControllerStates[] PostProcessingOptionSet = new EnumControllerStates[] { EnumControllerStates.DataSearch, EnumControllerStates.SaveDiscoveryData, EnumControllerStates.SaveProcessedData, EnumControllerStates.BackAction };

            _StateMachine = new Dictionary<EnumControllerStates, EnumControllerStates[]>
            {
                { EnumControllerStates.Main, new EnumControllerStates[]{ EnumControllerStates.AddDeviceDefinition, EnumControllerStates.LoadDiscoveryProfile, EnumControllerStates.Exit } },
                { EnumControllerStates.AddDeviceDefinition, CommonDefsOptionSet},
                { EnumControllerStates.ShowDeviceDefinitions, CommonDefsOptionSet},
                { EnumControllerStates.EditDeviceDefinition, CommonDefsOptionSet},
                { EnumControllerStates.DeleteDeviceDefinition, CommonDefsOptionSet},
                { EnumControllerStates.AddProcessDefinition, CommonDefsOptionSet},
                { EnumControllerStates.ShowProcessDefinitions, CommonDefsOptionSet},
                { EnumControllerStates.EditProcessDefinition, CommonDefsOptionSet},
                { EnumControllerStates.DeleteProcessDefinition, CommonDefsOptionSet},
                //MJE - Pending of final definition
                { EnumControllerStates.LoadDiscoveryProfile, CommonDefsOptionSet },
                { EnumControllerStates.RunProcess, PostProcessingOptionSet },
                { EnumControllerStates.DataSearch, PostProcessingOptionSet },
                { EnumControllerStates.SaveDiscoveryData, PostProcessingOptionSet },
                { EnumControllerStates.SaveProcessedData, PostProcessingOptionSet },
                { EnumControllerStates.BackAction, null},
                { EnumControllerStates.Exit, null}
            };
        }

        #endregion
    }
}
