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

        private Dictionary<EnumControllerStates, Action> StateHandlers { get; set; }
        private Dictionary<EnumControllerStates, string> StateLabels { get; set; }
        private string[] ShowDataMode, ShowDataOptions;

        #endregion

        #region Observer Implementation

        public void OnNext(ISNMPModelDTO value)
        {
            PromptModelDTO(value.ChangedObject.First, value.ChangedObject.Second);
        }

        public void OnError(Exception error)
        {
            //ViewHelper.RedirectConsoleToFile(true);
            Console.WriteLine($"Exception {error.GetType().Name}:\n" +
                              $"\t- Message: {error.Message}\n" +
                              $"\t- HResult: {error.HResult}\n" +
                              $"\t- Inner exception: {error.InnerException}\n");
            //ViewHelper.RedirectConsoleToFile(false);
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

            Console.WriteLine();
        }

        #endregion

        #region State Handlers

        private void NextActionHandle()
        {
            ShowStateCommands();
            GetStateCommand();
            StateHandlers[_controller.GetCurrentState()].Invoke();
        }

        private void ShowStateCommands()
        {
            Console.WriteLine("----- Available commands -----\n");

            EnumControllerStates[] Cmds = _controller.GetStateCommands();

            for (int i = 0; i < Cmds.Length; i++)
            {
                Console.WriteLine($"{i} - {StateLabels[Cmds[i]]}");
            }

            Console.WriteLine();
        }

        private void GetStateCommand()
        {
            int OnGoingState;
            bool wrongInput = false;

            do
            {
                Console.Write("Select option: ");

                wrongInput = !int.TryParse(Console.ReadLine(), out OnGoingState);

                if (wrongInput)
                {
                    Console.WriteLine("ERROR: Input values are not a number.");
                }
                else if (!_controller.ChangeState(OnGoingState))
                {
                    Console.WriteLine("ERROR: Selected option not available");
                    wrongInput = true;
                }
            } while (wrongInput);

            Console.WriteLine();
        }

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

            NextActionHandle();
        }

        private void ShowDeviceSettings()
        {
            string skey = null;
            IList<ISNMPDeviceSettingDTO> sresult = null;

            //Ask for key
            Console.Write("Insert device definition name. If blank, full list will be retrieved: ");
            skey = Console.ReadLine();
            Console.WriteLine();

            //Pull data and prompt
            sresult = _controller.GetSNMPDeviceSetting(skey);
            Console.WriteLine($"****** {sresult.Count} results found. ******\n");

            foreach (ISNMPDeviceSettingDTO DeviceSetItem in sresult)
            {
                ShowData(DeviceSetItem);
            }

            NextActionHandle();
        }

        private void EditDeviceSetting()
        {
            bool wrongInput = true;
            string skey = null;
            ISNMPDeviceSettingDTO sresult = null;
            string settingname, initialIPAndMask, finalIPAndMask, SNMPuser;

            //Ask for key
            do
            {
                Console.Write("Insert device definition name: ");
                skey = Console.ReadLine();

                wrongInput = string.IsNullOrWhiteSpace(skey);
                if (wrongInput)
                {
                    Console.WriteLine("Insert non-empty value.");
                }
            }
            while (wrongInput);

            Console.WriteLine();

            //Pull existing data and editing
            sresult = _controller.GetSNMPDeviceSetting(skey).FirstOrDefault();

            ShowData(sresult);

            if(sresult != null)
            {
                //Redefine values
                //MJE - Pending rencapsule methods of each getting step of processes
                Console.WriteLine("Insert values to be editted. Insert null-values for keeping previous ones.");

                Console.Write("Device definition name: ");
                settingname = Console.ReadLine();
                settingname = string.IsNullOrWhiteSpace(settingname) ? sresult.ID : settingname;

                Console.Write("Initial IP/mask: ");
                initialIPAndMask = Console.ReadLine();
                initialIPAndMask = string.IsNullOrWhiteSpace(initialIPAndMask) ? $"{sresult.InitialIP}/{sresult.NetworkMask}" : initialIPAndMask;

                Console.Write("Final IP/mask: ");
                finalIPAndMask = Console.ReadLine();
                finalIPAndMask = string.IsNullOrWhiteSpace(finalIPAndMask) ? $"{sresult.FinalIP}/{sresult.NetworkMask}" : finalIPAndMask;

                Console.Write("SNMP community user (V2): ");
                SNMPuser = Console.ReadLine();
                SNMPuser = string.IsNullOrWhiteSpace(SNMPuser) ? sresult.CommunityString : SNMPuser;

                Console.WriteLine();

                _controller.EditDevice(settingname, sresult.ID, initialIPAndMask, finalIPAndMask, SNMPuser);
            }
            else
            {
                Console.WriteLine("Device Setting not found.\n");
            }

            NextActionHandle();
        }

        private void DeleteDevice()
        {
            bool wrongInput = true, confirmdeletion = false;
            string skey = null, deletioncmd = null;
            ISNMPDeviceSettingDTO sresult = null;

            //Ask for key
            do
            {
                Console.Write("Insert device definition name: ");
                skey = Console.ReadLine();

                wrongInput = string.IsNullOrWhiteSpace(skey);
                if (wrongInput)
                {
                    Console.WriteLine("Insert non-empty value.");
                }
            }
            while (wrongInput);

            wrongInput = true;
            Console.WriteLine();

            //Pull existing data and editing
            sresult = _controller.GetSNMPDeviceSetting(skey).FirstOrDefault();
            ShowData(sresult);

            if(sresult != null)
            {
                //Ask for confirmation
                do
                {
                    Console.Write("WARNING: Delete DeviceSetting implies wiping relationships with processing settings. Continue with deletion of this device setting? [Y/N] ");
                    deletioncmd = Console.ReadLine().ToUpper();

                    wrongInput = string.IsNullOrWhiteSpace(skey) || !(deletioncmd == "Y" || deletioncmd == "N");

                    if (wrongInput)
                    {
                        Console.WriteLine("Insert valid value.");
                    }
                    else
                    {
                        confirmdeletion = deletioncmd == "Y";
                    }
                }
                while (wrongInput);

                //Delete command
                if (confirmdeletion)
                {
                    _controller.DeleteDevice(skey);
                }
            }
            else
            {
                Console.WriteLine("Device Setting not found.\n");
            }

            NextActionHandle();
        }

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

            IList<ISNMPDeviceSettingDTO> settingList = _controller.GetSNMPDeviceSetting();
            SettingDefinitions = settingList.Select(x => x.ID).ToArray();

            for (int i = 0; i < SettingDefinitions.Length; i++)
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

            Console.WriteLine();

            //Check if all settings options selected
            if (optionInput == SettingDefinitions.Length)
            {
                foreach (string settingID in SettingDefinitions)
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

        private void ShowProcesses()
        {
            string skey = null;
            IList<ISNMPProcessStrategy> sresult = null;

            //Ask for key
            Console.Write("Insert process name. If blank, full list will be retrieved: ");
            skey = Console.ReadLine();
            Console.WriteLine();

            //Pull data and prompt
            sresult = _controller.GetSNMPProcessStrategy(skey);
            Console.WriteLine($"****** {sresult.Count} results found. ******\n");

            foreach (ISNMPProcessStrategy DeviceSetItem in sresult)
            {
                ShowData(DeviceSetItem);
            }

            NextActionHandle();
        }

        private void EditProcess()
        {
            string[] ProcessingOptions;
            bool wrongInput = true;
            int optionInput;
            EnumProcessingType selectedOption;
            string skey = null;
            ISNMPProcessStrategy sresult = null;

            //Ask for key
            do
            {
                Console.Write("Insert process definition name: ");
                skey = Console.ReadLine();

                wrongInput = string.IsNullOrWhiteSpace(skey);
                if (wrongInput)
                {
                    Console.WriteLine("Insert non-empty value.");
                }
            }
            while (wrongInput);

            Console.WriteLine();

            //Pull exiting data and editing
            sresult = _controller.GetSNMPProcessStrategy(skey).FirstOrDefault();
            ShowData(sresult);

            if(sresult != null)
            {
                //Redefine values
                //MJE - Pending rencapsule methods of each getting step of processes
                Console.WriteLine("Select processing option:");

                ProcessingOptions = Enum.GetNames(typeof(EnumProcessingType));
                for (int i = 0; i < ProcessingOptions.Length; i++)
                {
                    Console.WriteLine($"{i} - {ProcessingOptions[i]}");
                }

                do
                {
                    Console.Write("Select option: ");
                    wrongInput = !int.TryParse(Console.ReadLine(), out optionInput);
                    wrongInput = !Enum.TryParse<EnumProcessingType>(ProcessingOptions[optionInput], out selectedOption) & wrongInput;
                }
                while (wrongInput);

                wrongInput = false;
                Console.WriteLine();

                //Save data
                _controller.EditProcess(sresult.ProcessID, selectedOption);
            }
            else
            {
                Console.WriteLine("Process setting not found.\n");
            }

            NextActionHandle();
        }

        private void DeleteProcess()
        {
            bool wrongInput = true, confirmdeletion = false;
            string skey = null, deletioncmd = null;
            ISNMPProcessStrategy sresult = null;

            //Ask for key
            do
            {
                Console.Write("Insert process definition name: ");
                skey = Console.ReadLine();

                wrongInput = string.IsNullOrWhiteSpace(skey);
                if (wrongInput)
                {
                    Console.WriteLine("Insert non-empty value.");
                }
            }
            while (wrongInput);

            Console.WriteLine();

            //Pull exiting data and editing
            sresult = _controller.GetSNMPProcessStrategy(skey).FirstOrDefault();
            ShowData(sresult);

            if (sresult != null)
            {
                //Ask for confirmation
                do
                {
                    Console.Write("WARNING: Continue with deletion of this process setting? [Y/N]?");
                    deletioncmd = Console.ReadLine().ToUpper();

                    wrongInput = string.IsNullOrWhiteSpace(skey) && !(deletioncmd == "Y" || deletioncmd == "N");

                    if (wrongInput)
                    {
                        Console.WriteLine("Insert valid value.");
                    }
                    else
                    {
                        confirmdeletion = deletioncmd == "Y";
                    }
                }
                while (wrongInput);

                //Delete command
                if (confirmdeletion)
                {
                    _controller.DeleteProcess(sresult.ProcessID);
                }
            }
            else
            {
                Console.WriteLine("Process setting not found.\n");
            }

            NextActionHandle();
        }

        private void RunProcessMenu()
        {
            //Posible acitons
            Console.WriteLine("Running discovery for gathering raw data");
            _controller.RunDiscovery();

            Console.WriteLine();
            Console.WriteLine("Running processes using collected data\n");
            _controller.RunProcesses();

            NextActionHandle();
        }

        private void ShowDataMenu()
        {
            int optionInput;
            bool wrongInput = false;

            do
            {
                Console.Write("Select option: ");

                wrongInput = !int.TryParse(Console.ReadLine(), out optionInput);

                if (wrongInput)
                {
                    Console.WriteLine("ERROR: Input values are not a number.");
                }
                else if (optionInput >= ShowDataMode.Length)
                {
                    Console.WriteLine("ERROR: Selected option not available");
                    wrongInput = true;
                }
            } while (wrongInput);

            Console.WriteLine();

            if (optionInput == 0)
            {
                //  0) Show global data
            }
            else
            {
                //Get IP & search
                //IF exist results

                //Set specific query
                //  1.1) Device basic data
                //  1.2) Port inventory
                //  1.3) Port volumetry
                //  1.4) Port volumetry
                //  1.5) Connected devices 

                //Run specific showdatamethod o specific DTO method
            }

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

        private void PromptModelDTO(Type datatype, object data)
        {
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
            else if (datatype.Equals(typeof(IGlobalTopologyInfoDTO)))
            {
                ShowData((IGlobalTopologyInfoDTO)data);
            }
        }

        private void ShowData(ISNMPDeviceDataDTO data)
        {
            if (data != null)
            {
                //ViewHelper.RedirectConsoleToFile(true);
                //Console.WriteLine($"SNMP device {data.TargetIP}.\n");
                //ViewHelper.RedirectConsoleToFile(false);
            }
            
        }

        private void ShowData(ISNMPDeviceSettingDTO data)
        {
            if (data != null)
            {
                ViewHelper.RedirectConsoleToFile(true);
                Console.WriteLine($"SNMP setting \"{data.ID}\" with this definition:\n" +
                                  $"\t-Initial IP: {data.InitialIP}/{data.NetworkMask}\n" +
                                  $"\t-Final IP: {data.FinalIP}/{data.NetworkMask}\n" +
                                  $"\t-Community string: {data.CommunityString}\n");
                ViewHelper.RedirectConsoleToFile(false);
            }
        }

        private void ShowData(ISNMPProcessStrategy data)
        {
            if (data != null)
            {
                ViewHelper.RedirectConsoleToFile(true);
                Console.WriteLine($"Process setting \"{data.ProcessID}\" related to following Device Settings:\n" +
                              $"\t-{string.Join("\n\t-", data.TargetDeviceSettings.Select(x => x.ID))}.\n");
                ViewHelper.RedirectConsoleToFile(false);
            }
        }

        private void ShowData(IOIDSettingDTO data)
        {
            if(data != null)
            {
                ViewHelper.RedirectConsoleToFile(true);
                Console.WriteLine($"OID setting {data.ID} with this definition:\n" +
                              $"\t-Initial OID: {data.InitialOID}\n" +
                              $"\t-Final OID: {data.FinalOID}\n" +
                              $"\t-Inclusive: {data.InclusiveInterval}\n");
                ViewHelper.RedirectConsoleToFile(false);
            }
        }

        private void ShowData(ISNMPRawEntryDTO data)
        {
            if(data != null)
            {
                //ViewHelper.RedirectConsoleToFile(true);
                //Console.WriteLine($"OID entry of {data.RegardingObject.TargetIP}. Identifier: {data.OID}. DataType: {data.DataType}. Value: {data.ValueData}.\n");
                //ViewHelper.RedirectConsoleToFile(false);
            }
        }

        private void ShowData(ISNMPProcessedValueDTO data)
        {
            if(data != null)
            {
                //PromptBasicInfo
                ShowData((IDiscoveredBasicInfo)data.Data);

                //PromptSpecificTypeInfo
                if (data.DataType.Equals(typeof(IDeviceTopologyInfoDTO)))
                {
                    ShowData((IDeviceTopologyInfoDTO)data.Data);
                }
            }
        }

        private void ShowData(IDiscoveredBasicInfo data)
        {
            ViewHelper.RedirectConsoleToFile(true);
            Console.WriteLine($"Processed device basic data:\n" +
                                $"\t-Device IP/mask: {data.DeviceIPAndMask}\n" +
                                $"\t-Device MAC: {data.DeviceMAC}\n" +
                                $"\t-Device name: {data.DeviceName}\n" +
                                $"\t-Device Type: {data.DeviceType}\n" +
                                $"\t-Location: {data.Location}\n" +
                                $"\t-Description: {data.Description}\n" +
                                $"\t-OSI Implemented Layers: {data.OSIImplementedLayers}\n");
            ViewHelper.RedirectConsoleToFile(false);
        }

        private void ShowData(IDeviceTopologyInfoDTO data)
        {
            ViewHelper.RedirectConsoleToFile(true);

            ShowPortInventory(data);
            ShowMACVolumetry(data);
            ShowMACsByPort(data);
            ShowDirectNeighbours(data);

            Console.WriteLine();

            ViewHelper.RedirectConsoleToFile(false);
        }

        private void ShowDirectNeighbours(IDeviceTopologyInfoDTO data)
        {
            Console.WriteLine("\nComputed direct neighbours:\n");
            Console.WriteLine("Port ID \t MAC Address \t IP Address");

            foreach (KeyValuePair<string, IDictionary<string, string>> computedneigh in data.DeviceDirectNeighbours)
            {
                foreach (KeyValuePair<string, string> addrrlist in computedneigh.Value)
                {
                    Console.WriteLine($"{computedneigh.Key} \t {addrrlist.Key} \t {addrrlist.Value}");
                }
            }
        }

        private void ShowMACsByPort(IDeviceTopologyInfoDTO data)
        {
            if (data.DeviceType == EnumDeviceType.RT || data.DeviceType == EnumDeviceType.SW || data.DeviceType == EnumDeviceType.AP)
            {
                //Learned MACs by port
                Console.WriteLine("\nLearned MACs by port:\n");
                Console.WriteLine("Port ID \t MAC Address \t IP Address");

                foreach (KeyValuePair<string, IDictionary<string, string>> portlearned in data.PortLearnedAddresses)
                {
                    foreach (KeyValuePair<string, string> maclist in portlearned.Value)
                    {
                        Console.WriteLine($"{portlearned.Key} \t {maclist.Key} \t {maclist.Value}");
                    }
                }
            }
        }

        private void ShowMACVolumetry(IDeviceTopologyInfoDTO data)
        {
            if (data.DeviceType == EnumDeviceType.RT || data.DeviceType == EnumDeviceType.SW || data.DeviceType == EnumDeviceType.AP)
            {
                //Volumetry MACs by port
                Console.WriteLine("\nVolumetry MACs by port:\n");
                Console.WriteLine("Port ID \t Quantity");

                foreach (KeyValuePair<string, IDictionary<string, string>> portlearned in data.PortLearnedAddresses)
                {
                    Console.WriteLine($"{portlearned.Key} \t {portlearned.Value.Count}");
                }
            }
        }

        private void ShowPortInventory(IDeviceTopologyInfoDTO data)
        {
            Console.WriteLine("\nPort inventory by internal ID:\n");
            Console.WriteLine("{0,-40} {1,-40} {2,-40} {3,-40} {4,-40} {5,-40}", "Port ID", "Port Name", "MAC Address", "Port Type", "Port Referer", "VLAN");

            foreach (KeyValuePair<string, string> MACPort in data.PortMACAddress)
            {
                List<string> relVLANs;
                bool existVLAN = data.PortVLANMapping.TryGetValue(MACPort.Key, out relVLANs);
                string VLANList = existVLAN ? string.Join(",", relVLANs.Select(x => data.VLANInventory[x])) : string.Empty;

                Console.WriteLine($"{MACPort.Key,-40} {data.PortDescriptions[MACPort.Key],-40} {MACPort.Value,-40} {data.PortSettings[MACPort.Key].First,-40} {data.PortSettings[MACPort.Key].Second,-40} {VLANList,-40}");
            }
        }

        private void ShowData(IGlobalTopologyInfoDTO data)
        {
            ViewHelper.RedirectConsoleToFile(true);

            Console.WriteLine("\nGlobal Device Mapping Matrix:\n");
            Console.WriteLine("{0,-40} {1,-40} {2,-40} {3,-40} {4,-40} {5,-40} {6,-40} {7,-40} {8,-40} {9,-40}", "Dev Type Orig", "IP Orig", "MAC Orig", "Port Orig", "Descrip Orig", "Dev Type Dest", "IP Dest", "MAC Dest", "Port Dest", "Descrip Dest");

            foreach (KeyValuePair<Tuple<string, string, string, string, string>, IList<Tuple<string, string, string, string, string>>> TopoOrigEntry in data.TopologyMatrix)
            {
                foreach (Tuple<string, string, string, string, string> TopoDestEntry in TopoOrigEntry.Value)
                {
                    Console.WriteLine($"{TopoOrigEntry.Key.Item1,-40}" +
                                 $"{TopoOrigEntry.Key.Item2,-40}" +
                                 $"{TopoOrigEntry.Key.Item3,-40}" +
                                 $"{TopoOrigEntry.Key.Item4,-40}" +
                                 $"{TopoOrigEntry.Key.Item5,-40}" +
                                 $"{TopoDestEntry.Item1,-40}" +
                                 $"{TopoDestEntry.Item2,-40}" +
                                 $"{TopoDestEntry.Item3,-40}" +
                                 $"{TopoDestEntry.Item4,-40}" +
                                 $"{TopoDestEntry.Item5,-40}");
                }
            }

            ViewHelper.RedirectConsoleToFile(false);
        }

        #endregion

        #region Initializers - Finalizers

        public SNMPDiscoveryView(ISNMPModelDTO Model, ISNMPDiscoveryController Controller)
        {
            _controller = Controller;
            _observeableSubscription = Model.Subscribe(this);
            _controller.OnInvalidInputs += ControllerErrorMessageHandler;

            InitView();
        }

        private void InitView()
        {
            StateHandlers = new Dictionary<EnumControllerStates, Action>()
            {
                { EnumControllerStates.Main, NextActionHandle },
                { EnumControllerStates.LoadDiscoveryProfile, LoadDataMenu },
                { EnumControllerStates.AddDeviceDefinition, DefineDevice },
                { EnumControllerStates.ShowDeviceDefinitions, ShowDeviceSettings},
                { EnumControllerStates.EditDeviceDefinition, EditDeviceSetting},
                { EnumControllerStates.DeleteDeviceDefinition, DeleteDevice },
                { EnumControllerStates.AddProcessDefinition, DefineProcess },
                { EnumControllerStates.ShowProcessDefinitions, ShowProcesses },
                { EnumControllerStates.EditProcessDefinition, EditProcess },
                { EnumControllerStates.DeleteProcessDefinition, DeleteProcess },
                { EnumControllerStates.RunProcess, RunProcessMenu },
                { EnumControllerStates.DataSearch, ShowDataMenu },
                { EnumControllerStates.SaveDiscoveryData, SaveDiscoveryDataMenu},
                { EnumControllerStates.SaveProcessedData, SaveProcessedDataMenu},
                { EnumControllerStates.BackAction, null},
                { EnumControllerStates.Exit, ExitMenu }
            };

            StateLabels = new Dictionary<EnumControllerStates, string>()
            {
                { EnumControllerStates.Main, "Main" },
                { EnumControllerStates.LoadDiscoveryProfile, "Load devices and process profile" },
                { EnumControllerStates.AddDeviceDefinition, "Add device settings" },
                { EnumControllerStates.ShowDeviceDefinitions, "Show device settings" },
                { EnumControllerStates.EditDeviceDefinition, "Edit device setting" },
                { EnumControllerStates.DeleteDeviceDefinition, "Delete device setting" },
                { EnumControllerStates.AddProcessDefinition, "Add processing functions" },
                { EnumControllerStates.ShowProcessDefinitions, "Show processing functions" },
                { EnumControllerStates.EditProcessDefinition, "Edit processing function" },
                { EnumControllerStates.DeleteProcessDefinition, "Delete processing function" },
                { EnumControllerStates.RunProcess, "Run processes" },
                { EnumControllerStates.DataSearch, "Data search functions" },
                { EnumControllerStates.SaveDiscoveryData, "Save discovery data"},
                { EnumControllerStates.SaveProcessedData, "Save processed data"},
                { EnumControllerStates.BackAction, "Back to previous action"},
                { EnumControllerStates.Exit, "Exit application" }
            };

            ShowDataMode = new string[] { "Show global data", "Show specific device data" };
            ShowDataOptions = new string[] { "Device basic data", "Port inventory", "MACs Learned By Port", "Port volumetry", "Connected devices" };

            Console.WriteLine("Welcome to ASPB network documentation tool.\n");
            StateHandlers[_controller.GetCurrentState()].Invoke();
        }

        #endregion
        
    }
}
