using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;
using SNMPDiscovery.Model.Services;
using SNMPDiscovery.Controller;
using System.Collections;
using System.IO;
using SNMPDiscovery.Model.Helpers;

namespace SNMPDiscovery.View
{
    public class SNMPDiscoveryView : IObserver<ISNMPModelDTO>
    {
        #region Private properties

        private ISNMPDiscoveryController _controller { get; set; }
        private IDisposable _observeableSubscription { get; set; }
        private bool _currentactionOK { get; set; }
        private Stack<EnumViewStates> StateHistory { get; set; }

        private Dictionary<EnumViewStates, Action> StateHandlers { get; set; }
        private Dictionary<EnumViewStates, string> CommandLabels { get; set; }
        private Dictionary<EnumViewStates, EnumViewStates[]> StateMachine { get; set; }

        //Mock for redirecting console to file
        private FileStream ostrm { get; set; }
        private StreamWriter writer { get; set; }
        private TextWriter oldOut { get; set; }

        #endregion

        #region Observer Implementation

        public void OnNext(ISNMPModelDTO value)
        {
            PromptDTOInfo(value.ChangedObject.First, value.ChangedObject.Second);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"Exception: {error.ToString()}\n");
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Controller Message Handler

        private void ControllerErrorMessageHandler(List<string> ListMsgs)
        {
            Console.WriteLine("Validation errors of input values:");

            foreach (var msg in ListMsgs)
            {
                Console.WriteLine($"\t - {msg}");
            }

            _currentactionOK = false;
        }

        #endregion

        #region Control Flow Methods

        private void Initialize()
        {
            _currentactionOK = true;
            StateHistory = new Stack<EnumViewStates>(new EnumViewStates[]{ EnumViewStates.Main });

            StateHandlers = new Dictionary<EnumViewStates, Action>()
            {
                { EnumViewStates.Main, NextActionHandle },
                { EnumViewStates.LoadDiscoveryProfile, LoadDataMenu },
                { EnumViewStates.AddDeviceDefinition, DefineDevice },
                { EnumViewStates.ShowDeviceDefinitions, ShowDevices},
                { EnumViewStates.EditDeviceDefinition, EditDevice},
                { EnumViewStates.DeleteDeviceDefinition, DeleteProcess },
                { EnumViewStates.AddProcessDefinition, DefineProcess },
                { EnumViewStates.ShowProcessDefinitions, ShowProcesses },
                { EnumViewStates.EditProcessDefinition, EditProcess },
                { EnumViewStates.DeleteProcessDefinition, DeleteProcess },
                { EnumViewStates.RunProcess, RunProcessMenu },
                { EnumViewStates.DataSearch, PromptDataMenu },
                { EnumViewStates.SaveDiscoveryData, SaveDiscoveryDataMenu},
                { EnumViewStates.SaveProcessedData, SaveProcessedDataMenu},
                { EnumViewStates.BackAction, null},
                { EnumViewStates.Exit, ExitMenu }
            };

            CommandLabels = new Dictionary<EnumViewStates, string>()
            {
                { EnumViewStates.Main, "Main" },
                { EnumViewStates.LoadDiscoveryProfile, "Load devices and process profile" },
                { EnumViewStates.AddDeviceDefinition, "Add device settings" },
                { EnumViewStates.ShowDeviceDefinitions, "Show device settings" },
                { EnumViewStates.EditDeviceDefinition, "Edit device setting" },
                { EnumViewStates.DeleteDeviceDefinition, "Delete device setting" },
                { EnumViewStates.AddProcessDefinition, "Add processing functions" },
                { EnumViewStates.ShowProcessDefinitions, "Show processing functions" },
                { EnumViewStates.EditProcessDefinition, "Edit processing function" },
                { EnumViewStates.DeleteProcessDefinition, "Delete processing function" },
                { EnumViewStates.RunProcess, "Run processes" },
                { EnumViewStates.DataSearch, "Data search functions" },
                { EnumViewStates.SaveDiscoveryData, "Save discovery data"},
                { EnumViewStates.SaveProcessedData, "Save processed data"},
                { EnumViewStates.BackAction, "Back to previous action"},
                { EnumViewStates.Exit, "Exit application" }
            };

            EnumViewStates[] CommonDefsOptionSet = new EnumViewStates[] { EnumViewStates.AddDeviceDefinition, EnumViewStates.ShowDeviceDefinitions, EnumViewStates.EditDeviceDefinition, EnumViewStates.DeleteDeviceDefinition, EnumViewStates.AddProcessDefinition, EnumViewStates.ShowProcessDefinitions, EnumViewStates.EditProcessDefinition, EnumViewStates.DeleteProcessDefinition, EnumViewStates.RunProcess, EnumViewStates.BackAction };
            EnumViewStates[] PostProcessingOptionSet = new EnumViewStates[] { EnumViewStates.DataSearch, EnumViewStates.SaveDiscoveryData, EnumViewStates.SaveProcessedData, EnumViewStates.BackAction };

            StateMachine = new Dictionary<EnumViewStates, EnumViewStates[]>
            {
                { EnumViewStates.Main, new EnumViewStates[]{ EnumViewStates.AddDeviceDefinition, EnumViewStates.LoadDiscoveryProfile, EnumViewStates.Exit } },
                { EnumViewStates.AddDeviceDefinition, CommonDefsOptionSet},
                { EnumViewStates.ShowDeviceDefinitions, CommonDefsOptionSet},
                { EnumViewStates.EditDeviceDefinition, CommonDefsOptionSet},
                { EnumViewStates.DeleteDeviceDefinition, CommonDefsOptionSet},
                { EnumViewStates.AddProcessDefinition, CommonDefsOptionSet},
                { EnumViewStates.ShowProcessDefinitions, CommonDefsOptionSet},
                { EnumViewStates.EditProcessDefinition, CommonDefsOptionSet},
                { EnumViewStates.DeleteProcessDefinition, CommonDefsOptionSet},
                //MJE - Pending of final definition
                { EnumViewStates.LoadDiscoveryProfile, CommonDefsOptionSet },
                { EnumViewStates.RunProcess, PostProcessingOptionSet },
                { EnumViewStates.DataSearch, PostProcessingOptionSet },
                { EnumViewStates.SaveDiscoveryData, PostProcessingOptionSet },
                { EnumViewStates.SaveProcessedData, PostProcessingOptionSet },
                { EnumViewStates.BackAction, null},
                { EnumViewStates.Exit, null}
            };

            Console.WriteLine("Welcome to ASPB network documentation tool.\n");
            StateHandlers[StateHistory.Peek()].Invoke();
        }

        private void NextActionHandle()
        {
            if (!_currentactionOK)
            {
                StateHistory.Pop();
                _currentactionOK = true;
            }

            ShowStateCommands();
            GetStateCommand();
            StateHandlers[StateHistory.Peek()].Invoke();
        }

        private void ShowStateCommands()
        {
            Console.WriteLine("----- Available commands -----\n");

            for (int i = 0; i < StateMachine[StateHistory.Peek()].Length; i++)
            {
                Console.WriteLine($"{i} - {CommandLabels[StateMachine[StateHistory.Peek()][i]]}");
            }

            Console.WriteLine();
        }

        private void GetStateCommand()
        {
            int GoingState;
            bool wrongInput = false;

            do
            {
                Console.Write("Select option: ");
                                
                wrongInput = !int.TryParse(Console.ReadLine(), out GoingState);

                if (wrongInput)
                {
                    Console.WriteLine("ERROR: Input values are not a number.");
                }
                else if (GoingState >= StateMachine[StateHistory.Peek()].Length)
                {
                    Console.WriteLine("ERROR: Selected option not available");
                    wrongInput = true;
                }
            } while (wrongInput);

            Console.WriteLine();

            if(StateMachine[StateHistory.Peek()][GoingState] == EnumViewStates.BackAction)
            {
                StateHistory.Pop();
                NextActionHandle();
            }
            else
            {
                StateHistory.Push(StateMachine[StateHistory.Peek()][GoingState]);
            }
        }

        #endregion

        #region State Handlers

        private void LoadDataMenu()
        {
            //Posible acitons

            NextActionHandle();
        }

        private void DefineDevice()
        {
            string settingname, initialIP, finalIP, SNMPuser;

            Console.Write("Device definition name: ");
            settingname = Console.ReadLine();
            Console.Write("Initial IP/mask: ");
            initialIP = Console.ReadLine();
            Console.Write("Final IP/mask: ");
            finalIP = Console.ReadLine();
            Console.Write("SNMP community user (V2): ");
            SNMPuser = Console.ReadLine();
            Console.WriteLine();

            _controller.DefineDevices(settingname, initialIP, finalIP, SNMPuser);
            //_controller.DefineDevice("ColecciónSwitches", "192.168.1.42", "192.168.1.51", "public");

            NextActionHandle();
        }

        private void ShowDevices() { }

        private void EditDevice() { }

        private void DeleteDevice() { }

        private void DefineProcess()
        {
            string[] ProcessingOptions, SettingDefinitions;
            int optionInput;
            EnumProcessingType selectedOption;
            bool wrongOption = false;

            Console.WriteLine("Select processing option:");

            ProcessingOptions = Enum.GetNames(typeof(EnumProcessingType));
            for (int i = 0; i < ProcessingOptions.Length; i++)
            {
                Console.WriteLine($"{i} - {ProcessingOptions[i]}");
            }

            do
            {
                Console.Write("Select option: ");
                wrongOption = !int.TryParse(Console.ReadLine(), out optionInput);
                wrongOption = !Enum.TryParse<EnumProcessingType>(ProcessingOptions[optionInput], out selectedOption) & wrongOption;
            }
            while (wrongOption);

            wrongOption = false;
            Console.WriteLine();
            Console.WriteLine("Select settings to apply this processing:");

            IList<ISNMPDeviceSettingDTO> settingList = (List<ISNMPDeviceSettingDTO>) (_controller.PullDataList(typeof(ISNMPDeviceSettingDTO)));
            SettingDefinitions = settingList.Select(x => x.ID).ToArray();

            for(int i = 0; i < SettingDefinitions.Length; i++)
            {
                Console.WriteLine($"{i} - {SettingDefinitions[i]}");
            }
            //All settings option
            Console.WriteLine($"{SettingDefinitions.Length} - All settings");

            do
            {
                Console.Write("Select setting: ");
                wrongOption = !int.TryParse(Console.ReadLine(), out optionInput);
                wrongOption = optionInput > SettingDefinitions.Length && optionInput < 0 && wrongOption;
            }
            while (wrongOption);

            //Check if all settings options selected
            if(optionInput == SettingDefinitions.Length)
            {
                foreach(string settingID in SettingDefinitions)
                {
                    _controller.DefineProcesses(settingID, selectedOption);
                }
            }
            else
            {
                _controller.DefineProcesses(SettingDefinitions[optionInput], selectedOption);
            }

            NextActionHandle();
        }

        private void ShowProcesses() { }

        private void EditProcess() { }

        private void DeleteProcess() { }

        private void RunProcessMenu()
        {
            //Posible acitons
            Console.WriteLine("Running discovery for gathering raw data");
            _controller.RunDiscovery();

            Console.WriteLine();
            Console.WriteLine("Running processes using collected data");
            _controller.RunProcesses();

            NextActionHandle();
        }

        private void PromptDataMenu()
        {
            //Posible acitons

            NextActionHandle();
        }

        private void SaveDiscoveryDataMenu()
        {
            //Posible acitons

            NextActionHandle();
        }

        private void SaveProcessedDataMenu()
        {
            //Posible acitons

            NextActionHandle();
        }

        private void ExitMenu()
        {
            Console.WriteLine("Exiting of application\n");
            Environment.Exit(0);
        }

        #endregion

        #region Data Visualization

        private void PromptDTOInfo(Type datatype, object data)
        {
            RedirectToFile(true);

            if (datatype.Equals(typeof(ISNMPDeviceDataDTO)))
            {
                ShowData((ISNMPDeviceDataDTO)data);
            }
            else if (datatype.Equals(typeof(ISNMPDeviceSettingDTO)))
            {
                ShowData((ISNMPDeviceSettingDTO)data);
            }
            else if (datatype.Equals(typeof(ISNMPProcessStrategy)))
            {
                ShowData((ISNMPProcessStrategy)data);
            }
            else if (datatype.Equals(typeof(IOIDSettingDTO)))
            {
                ShowData((IOIDSettingDTO)data);
            }
            else if (datatype.Equals(typeof(ISNMPRawEntryDTO)))
            {
                ShowData((ISNMPRawEntryDTO)data);
            }
            else if (datatype.Equals(typeof(ISNMPProcessedValueDTO)))
            {
                ShowData((ISNMPProcessedValueDTO)data);
            }
            else
            {
            }

            RedirectToFile(false);
        }

        private void ShowData(ISNMPDeviceDataDTO data)
        {
            Console.WriteLine($"Added SNMP device {data.TargetIP}.\n");
        }

        private void ShowData(ISNMPDeviceSettingDTO data)
        {
            //MJE TEST
            Console.WriteLine($"Timestamp {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")}");
            Console.WriteLine();

            Console.WriteLine($"Added SNMP setting \"{data.ID}\" with this definition:\n" +
                              $"\t-Initial IP: {data.InitialIP}/{data.NetworkMask}\n" +
                              $"\t-Final IP: {data.FinalIP}/{data.NetworkMask}\n" +
                              $"\t-Community string: {data.CommunityString}\n");
        }

        private void ShowData(ISNMPProcessStrategy data)
        {
            Console.WriteLine($"Added process setting {data.ProcessID} related to following Device Settings: {string.Join(", ", data.TargetDevices.Select(x => x.ID))}.\n");
        }

        private void ShowData(IOIDSettingDTO data)
        {
            Console.WriteLine($"Added OID setting {data.ID} with this definition:\n" +
                              $"\t-Initial OID: {data.InitialOID}\n" +
                              $"\t-Final OID: {data.FinalOID}\n" +
                              $"\t-Inclusive: {data.InclusiveInterval}\n");
        }

        private void ShowData(ISNMPRawEntryDTO data)
        {
            Console.WriteLine($"Added OID data. Identifier: {data.OID}. DataType: {data.DataType}. Value: {data.ValueData}.\n");
        }

        private void ShowData(ISNMPProcessedValueDTO data)
        {
            //PromptBasicInfo
            IDiscoveredBasicInfo BasicInfoObj = (IDiscoveredBasicInfo)data.Data;

            Console.WriteLine($"Processed device basic data:\n" +
                              $"\t-Device name: {BasicInfoObj.DeviceName}\n" +
                              $"\t-Device Type: {BasicInfoObj.DeviceType}\n" +
                              $"\t-Location: {BasicInfoObj.Location}\n" +
                              $"\t-Description: {BasicInfoObj.Description}\n" +
                              $"\t-OSI Implemented Layers: {BasicInfoObj.OSIImplementedLayers}\n");

            //PromptSpecificTypeInfo
            if (data.DataType.Equals(typeof(IDeviceTopologyInfoDTO)))
            {
                IDeviceTopologyInfoDTO DataObj = (IDeviceTopologyInfoDTO)data.Data;

                //Port inventory by internal ID
                Console.WriteLine("\nPort inventory by internal ID:\n");
                Console.WriteLine("{0,-40} {1,-40} {2,-40} {3,-40} {4,-40} {5,-40}", "Port ID", "Port Name", "MAC Address", "Port Type", "Port Referer", "VLAN");

                foreach (KeyValuePair<string, string> MACPort in DataObj.PortMACAddress)
                {
                    List<string> relVLANs;
                    bool existVLAN = DataObj.PortVLANMapping.TryGetValue(MACPort.Key, out relVLANs);
                    string VLANList = existVLAN ? string.Join(",", relVLANs.Select(x => DataObj.VLANInventory[x])) : string.Empty;

                    Console.WriteLine($"{MACPort.Key,-40} {DataObj.PortInventory[MACPort.Key],-40} {MACPort.Value,-40} {DataObj.PortSettings[MACPort.Key].First,-40} {DataObj.PortSettings[MACPort.Key].Second,-40} {VLANList,-40}");
                }

                //Volumetry MACs by port
                Console.WriteLine("\nVolumetry MACs by port:\n");
                Console.WriteLine("Port ID \t Quantity");
                
                foreach (KeyValuePair<string, IDictionary<string, string>> portlearned in DataObj.PortLearnedAddresses)
                {
                    Console.WriteLine($"{portlearned.Key} \t {portlearned.Value.Count}");
                }

                //Learned MACs by port
                //Console.WriteLine("\nLearned MACs by port:\n");
                //Console.WriteLine("Port ID \t MAC Address \t IP Address");
                //
                //foreach (KeyValuePair<string, IDictionary<string,string>> portlearned in DataObj.PortLearnedAddresses)
                //{
                //    foreach (KeyValuePair<string,string> maclist in portlearned.Value)
                //    {
                //        Console.WriteLine($"{portlearned.Key} \t {maclist.Key} \t {maclist.Value}");
                //    }
                //}

                //Direct Neighbours
                Console.WriteLine("\nComputed direct neighbours:\n");
                Console.WriteLine("Port ID \t MAC Address \t IP Address");

                foreach (KeyValuePair<string, IDictionary<string, string>> computedneigh in DataObj.DeviceDirectNeighbours)
                {
                    foreach (KeyValuePair<string, string> addrrlist in computedneigh.Value)
                    {
                        Console.WriteLine($"{computedneigh.Key} \t {addrrlist.Key} \t {addrrlist.Value}");
                    }
                }
                Console.WriteLine();

                //MJE TEST
                Console.WriteLine($"Timestamp {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")}");
                Console.WriteLine();
            }
        }

        #endregion

        #region Constructor and destructor

        public SNMPDiscoveryView(ISNMPModelDTO Model, ISNMPDiscoveryController Controller)
        {
            _controller = Controller;
            _observeableSubscription = Model.Subscribe(this);
            _controller.OnInvalidInputs += ControllerErrorMessageHandler;

            Initialize();
        }

        //Mock for disposing redirection to file
        ~SNMPDiscoveryView()
        {
            //RedirectToFile(false);
        }

        #endregion

        //Mock for redirecting console to file
        private void RedirectToFile(bool activate)
        {
            if (activate)
            {
                oldOut = Console.Out;
                try
                {
                    ostrm = new FileStream("./Redirect.txt", FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(ostrm);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot open Redirect.txt for writing");
                    Console.WriteLine(e.Message);
                    return;
                }
                Console.SetOut(writer);
            }
            else
            {
                Console.SetOut(oldOut);
                writer.Close();
                ostrm.Close();
            }
        }
    }
}
