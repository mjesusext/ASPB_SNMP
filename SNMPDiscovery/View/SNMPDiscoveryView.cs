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
        private ISNMPDiscoveryController _controller { get; set; }
        private IDisposable _observeableSubscription { get; set; }
        private bool _currentactionOK;
        private Stack<EnumViewStates> StateHistory;

        private Dictionary<EnumViewStates, Action> StateHandlers;
        private Dictionary<EnumViewStates, string> CommandLabels;
        private Dictionary<EnumViewStates, EnumViewStates[]> StateMachine;

        //Mock for redirecting console to file
        private FileStream ostrm;
        private StreamWriter writer;
        private TextWriter oldOut;

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

        #region Private Methods

        private void Initialize()
        {
            _currentactionOK = true;
            StateHistory = new Stack<EnumViewStates>(new EnumViewStates[]{ EnumViewStates.Main });

            StateHandlers = new Dictionary<EnumViewStates, Action>()
            {
                { EnumViewStates.Main, NextActionHandle },
                { EnumViewStates.DeviceDefinition, DefineDevice },
                { EnumViewStates.LoadDiscoveryData, LoadDataMenu },
                { EnumViewStates.ProcessSelection, ProcessingMenu },
                { EnumViewStates.ProcessExecution, RunProcessMenu },
                { EnumViewStates.PullData, PromptDataMenu },
                { EnumViewStates.SaveDiscoveryData, SaveDiscoveryDataMenu},
                { EnumViewStates.SaveProcessedData, SaveProcessedDataMenu},
                { EnumViewStates.BackAction, null},
                { EnumViewStates.Exit, ExitMenu}
            };

            CommandLabels = new Dictionary<EnumViewStates, string>()
            {
                { EnumViewStates.Main, "Main" },
                { EnumViewStates.DeviceDefinition, "Define devices" },
                { EnumViewStates.LoadDiscoveryData, "Load existing SNMP data discovery" },
                { EnumViewStates.ProcessSelection, "Select processing functions" },
                { EnumViewStates.ProcessExecution, "Run processes" },
                { EnumViewStates.PullData, "Prompt data" },
                { EnumViewStates.SaveDiscoveryData, "Save discovery data"},
                { EnumViewStates.SaveProcessedData, "Save processed data"},
                { EnumViewStates.BackAction, "Back to previous action"},
                { EnumViewStates.Exit, "Exit application" }
            };

            StateMachine = new Dictionary<EnumViewStates, EnumViewStates[]>
            {
                { EnumViewStates.Main, new EnumViewStates[]{ EnumViewStates.DeviceDefinition, EnumViewStates.LoadDiscoveryData, EnumViewStates.Exit } },
                { EnumViewStates.DeviceDefinition, new EnumViewStates[]{ EnumViewStates.DeviceDefinition, EnumViewStates.ProcessSelection , EnumViewStates.BackAction} },
                { EnumViewStates.LoadDiscoveryData, new EnumViewStates[]{ EnumViewStates.ProcessSelection, EnumViewStates.BackAction } },
                { EnumViewStates.ProcessSelection, new EnumViewStates[]{ EnumViewStates.ProcessSelection, EnumViewStates.ProcessExecution, EnumViewStates.BackAction } },
                { EnumViewStates.ProcessExecution, new EnumViewStates[]{ EnumViewStates.PullData, EnumViewStates.BackAction } },
                { EnumViewStates.PullData, new EnumViewStates[]{ EnumViewStates.PullData, EnumViewStates.SaveDiscoveryData, EnumViewStates.SaveProcessedData, EnumViewStates.BackAction } },
                { EnumViewStates.SaveDiscoveryData, new EnumViewStates[]{ EnumViewStates.PullData, EnumViewStates.SaveDiscoveryData, EnumViewStates.SaveProcessedData, EnumViewStates.BackAction }},
                { EnumViewStates.SaveProcessedData, new EnumViewStates[]{ EnumViewStates.PullData, EnumViewStates.SaveDiscoveryData, EnumViewStates.SaveProcessedData, EnumViewStates.BackAction }},
                { EnumViewStates.BackAction, null},
                { EnumViewStates.Exit, null}
            };

            Console.WriteLine("Welcome to ASPB network documentation tool.\n");
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

        private void LoadDataMenu()
        {
            //Posible acitons

            NextActionHandle();
        }

        private void ProcessingMenu()
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

            IList<ISNMPSettingDTO> settingList = (List<ISNMPSettingDTO>) (_controller.PullDataList(typeof(ISNMPSettingDTO)));
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

        private void PromptDTOInfo(Type datatype, object data)
        {
            RedirectToFile(true);

            if (datatype.Equals(typeof(ISNMPDeviceDTO)))
            {
                ShowData((ISNMPDeviceDTO)data);
            }
            else if (datatype.Equals(typeof(ISNMPSettingDTO)))
            {
                ShowData((ISNMPSettingDTO)data);
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

        private void ShowData(ISNMPDeviceDTO data)
        {
            Console.WriteLine($"Added SNMP device {data.TargetIP}.\n");
        }

        private void ShowData(ISNMPSettingDTO data)
        {
            Console.WriteLine($"Added SNMP setting \"{data.ID}\" with this definition:\n" +
                              $"\t-Initial IP: {data.InitialIP}/{data.NetworkMask}\n" +
                              $"\t-Final IP: {data.FinalIP}/{data.NetworkMask}\n" +
                              $"\t-Community string: {data.CommunityString}\n");
        }

        private void ShowData(ISNMPProcessStrategy data)
        {
            Console.WriteLine($"Added process setting {data.ProcessID} related to {data.RegardingSetting}.\n");
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
            if (data.DataType.Equals(typeof(ITopologyInfoDTO)))
            {
                ITopologyInfoDTO DataObj = (ITopologyInfoDTO)data.Data;

                Console.WriteLine("\nPort inventory by internal ID :\n");
                Console.WriteLine("{0,-40} {1,-40} {2,-40} {3,-40} {4,-40} {5,-40}", "Port ID", "Port Name", "MAC Address", "Port Type", "Port Referer", "VLAN");

                //Further details of processing
                foreach (KeyValuePair<string, string> MACPort in DataObj.PortMACAddress)
                {
                    List<string> relVLANs;
                    bool existVLAN = DataObj.PortVLANMapping.TryGetValue(MACPort.Key, out relVLANs);
                    string VLANList = existVLAN ? string.Join(",", relVLANs.Select(x => DataObj.VLANInventory[x])) : string.Empty;

                    Console.WriteLine($"{MACPort.Key,-40} {DataObj.PortInventory[MACPort.Key],-40} {MACPort.Value,-40} {DataObj.PortSettings[MACPort.Key].First,-40} {DataObj.PortSettings[MACPort.Key].Second,-40} {VLANList,-40}");
                }

                Console.WriteLine("\nVolumetry MACs by port:\n");
                Console.WriteLine("Port ID \t Quantity");

                //Further details of processing
                foreach (KeyValuePair<string, IDictionary<string, string>> portlearned in DataObj.PortLearnedAddresses)
                {
                    Console.WriteLine($"{portlearned.Key} \t {portlearned.Value.Count}");
                }

                Console.WriteLine("\nLearned MACs by port:\n");
                Console.WriteLine("Port ID \t MAC Address \t IP Address");

                //Further details of processing
                foreach (KeyValuePair<string, IDictionary<string,string>> portlearned in DataObj.PortLearnedAddresses)
                {
                    foreach (KeyValuePair<string,string> maclist in portlearned.Value)
                    {
                        Console.WriteLine($"{portlearned.Key} \t {maclist.Key} \t {maclist.Value}");
                    }
                }
            }
        }

        #endregion
    }
}
