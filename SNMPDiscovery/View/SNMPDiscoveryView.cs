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

        //Mock for redirecting console to file
        private FileStream ostrm { get; set; }
        private StreamWriter writer { get; set; }
        private TextWriter oldOut { get; set; }

        #endregion

        #region Observer Implementation

        public void OnNext(ISNMPModelDTO value)
        {
            PromptModelDTO(value.ChangedObject.First, value.ChangedObject.Second);
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
            sresult = (List<ISNMPDeviceSettingDTO>)_controller.PullDataList(typeof(ISNMPDeviceSettingDTO), string.IsNullOrWhiteSpace(skey) ? null : skey);
            Console.WriteLine($"{sresult.Count} results found.");

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
            string settingname, initialIP, finalIP, SNMPuser;

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
            sresult = ((List<ISNMPDeviceSettingDTO>)_controller.PullDataList(typeof(ISNMPDeviceSettingDTO), skey))?[0];
            ShowData(sresult);

            //Redefine values
            //MJE - Pending rencapsule methods of each getting step of processes
            Console.WriteLine("Insert values to be editted. Insert null-values for keeping previous ones");

            Console.Write("Device definition name: ");
            settingname = Console.ReadLine();
            settingname = string.IsNullOrWhiteSpace(settingname) ? sresult.ID : settingname;

            Console.Write("Initial IP/mask: ");
            initialIP = Console.ReadLine();
            initialIP = string.IsNullOrWhiteSpace(initialIP) ? sresult.InitialIP.ToString() : initialIP;

            Console.Write("Final IP/mask: ");
            finalIP = Console.ReadLine();
            finalIP = string.IsNullOrWhiteSpace(finalIP) ? sresult.FinalIP.ToString() : finalIP;

            Console.Write("SNMP community user (V2): ");
            SNMPuser = Console.ReadLine();
            SNMPuser = string.IsNullOrWhiteSpace(SNMPuser) ? sresult.CommunityString : SNMPuser;

            Console.WriteLine();

            //_controller.DefineDevices(settingname, initialIP, finalIP, SNMPuser, sresult.ID);

            NextActionHandle();
        }

        private void DeleteDevice()
        {
            //Ask for key
            //Delete command
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

        private void ShowProcesses()
        {
            string skey = null;
            IList<ISNMPProcessStrategy> sresult = null;

            //Ask for key
            Console.Write("Insert process name. If blank, full list will be retrieved: ");
            skey = Console.ReadLine();
            Console.WriteLine();

            //Pull data and prompt
            sresult = (List<ISNMPProcessStrategy>)_controller.PullDataList(typeof(ISNMPProcessStrategy), string.IsNullOrWhiteSpace(skey) ? null : skey);
            Console.WriteLine($"{sresult.Count} results found.");

            foreach (ISNMPProcessStrategy DeviceSetItem in sresult)
            {
                ShowData(DeviceSetItem);
            }

            NextActionHandle();
        }

        private void EditProcess()
        {
            bool wrongInput = true;
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
            sresult = ((List<ISNMPProcessStrategy>)_controller.PullDataList(typeof(ISNMPProcessStrategy), skey))?[0];
            ShowData(sresult);

            //Redefine values
            Console.WriteLine("Insert values to be editted. Insert null-values for keeping previous ones");

            //MJE - Pending rencapsule methods of each getting step of processes
            
            //Save data
        }

        private void DeleteProcess()
        {
            //Ask for key
            //Delete command
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

        #endregion

        #region Data Visualization

        private void PromptModelDTO(Type datatype, object data)
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
            Console.WriteLine($"SNMP device {data.TargetIP}.\n");
        }

        private void ShowData(ISNMPDeviceSettingDTO data)
        {
            //MJE TEST
            Console.WriteLine($"Timestamp {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")}");
            Console.WriteLine();

            Console.WriteLine($"SNMP setting \"{data.ID}\" with this definition:\n" +
                              $"\t-Initial IP: {data.InitialIP}/{data.NetworkMask}\n" +
                              $"\t-Final IP: {data.FinalIP}/{data.NetworkMask}\n" +
                              $"\t-Community string: {data.CommunityString}\n");
        }

        private void ShowData(ISNMPProcessStrategy data)
        {
            Console.WriteLine($"Process setting {data.ProcessID} related to following Device Settings:\n" +
                              $"\t-{string.Join("\n\t-", data.TargetDevices.Select(x => x.ID))}.\n");
        }

        private void ShowData(IOIDSettingDTO data)
        {
            Console.WriteLine($"OID setting {data.ID} with this definition:\n" +
                              $"\t-Initial OID: {data.InitialOID}\n" +
                              $"\t-Final OID: {data.FinalOID}\n" +
                              $"\t-Inclusive: {data.InclusiveInterval}\n");
        }

        private void ShowData(ISNMPRawEntryDTO data)
        {
            Console.WriteLine($"OID entry. Identifier: {data.OID}. DataType: {data.DataType}. Value: {data.ValueData}.\n");
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
                { EnumControllerStates.DeleteDeviceDefinition, DeleteProcess },
                { EnumControllerStates.AddProcessDefinition, DefineProcess },
                { EnumControllerStates.ShowProcessDefinitions, ShowProcesses },
                { EnumControllerStates.EditProcessDefinition, EditProcess },
                { EnumControllerStates.DeleteProcessDefinition, DeleteProcess },
                { EnumControllerStates.RunProcess, RunProcessMenu },
                { EnumControllerStates.DataSearch, PromptDataMenu },
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

            Console.WriteLine("Welcome to ASPB network documentation tool.\n");
            StateHandlers[_controller.GetCurrentState()].Invoke();
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
