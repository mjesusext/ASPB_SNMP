using System;
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
        #region Public properties

        public IObserver<ISNMPModelDTO> View { get; set; }
        public ISNMPModelDTO Model { get; set; }
        public event Action<List<string>> OnInvalidInputs;

        #endregion

        #region Private properties

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

        public void LoadDiscoveryData() { }

        public void LoadDeviceDefinitions() { }

        public void DefineProcesses(string settingID, EnumProcessingType processType)
        {
            Model.BuildProcess(settingID, processType);
        }

        public void RunDiscovery()
        {
            Model.StartDiscovery();
        }

        public void RunProcesses()
        {
            foreach (ISNMPProcessStrategy proccess in Model.Processes.Values)
            {
                proccess.Run();
            }
        }

        public object PullDataList(Type dataType, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (dataType.Equals(typeof(ISNMPDeviceDataDTO)))
                {
                    return Model.DeviceData.Values.ToList();
                }
                else if (dataType.Equals(typeof(ISNMPDeviceSettingDTO)))
                {
                    return Model.DeviceSettings.Values.ToList();
                }
                else if (dataType.Equals(typeof(ISNMPProcessStrategy)))
                {
                    return Model.Processes.Values.ToList();
                }
                else if (dataType.Equals(typeof(IOIDSettingDTO)))
                {
                    return Model.Processes.Values.SelectMany(x => x.OIDSettings.Values).ToList();
                }
                else if (dataType.Equals(typeof(ISNMPRawEntryDTO)))
                {
                    return Model.DeviceData.Values.SelectMany(x => x.SNMPRawDataEntries.Values).ToList();
                }
                else if (dataType.Equals(typeof(ISNMPProcessedValueDTO)))
                {
                    return Model.DeviceData.Values.SelectMany(x => x.SNMPProcessedData.Values).ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (dataType.Equals(typeof(ISNMPDeviceDataDTO)))
                {
                    return new List<ISNMPDeviceDataDTO>(new[] { Model.DeviceData[key] });
                }
                else if (dataType.Equals(typeof(ISNMPDeviceSettingDTO)))
                {
                    return new List<ISNMPDeviceSettingDTO>(new[] { Model.DeviceSettings[key] });
                }
                else if (dataType.Equals(typeof(ISNMPProcessStrategy)))
                {
                    return new List<ISNMPProcessStrategy>(new[] { Model.Processes[key] });
                }
                else if (dataType.Equals(typeof(IOIDSettingDTO)))
                {
                    return new List<IOIDSettingDTO>(Model.Processes.Values.Select(x => x.OIDSettings[key]));
                }
                else if (dataType.Equals(typeof(ISNMPRawEntryDTO)))
                {
                    return new List<ISNMPRawEntryDTO>(Model.DeviceData.Values.Select(x => x.SNMPRawDataEntries[key]));
                }
                else if (dataType.Equals(typeof(ISNMPProcessedValueDTO)))
                {
                    return new List<ISNMPProcessedValueDTO>(Model.DeviceData.Values.Select(x => x.SNMPProcessedData[key]));
                }
                else
                {
                    return null;
                }
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
