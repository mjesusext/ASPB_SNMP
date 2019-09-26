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
            //Mock for redirecting console to file
            //RedirectToFile(true);

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
                    ostrm = new FileStream("./Redirect.txt", FileMode.Create, FileAccess.Write);
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
            foreach (KeyValuePair<Type, IList> DataCollection in value.ChangedObjects)
            {
                foreach(object DataItem in DataCollection.Value)
                {
                    PromptDTOInfo(DataCollection.Key, DataItem);
                }
            }
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
                { EnumViewStates.NotificationSetting, SetNotificationMenu },
                { EnumViewStates.PullData, PromptDataMenu },
                { EnumViewStates.SaveDiscoveryData, SaveDiscoveryDataMenu},
                { EnumViewStates.SaveProcessedData, SaveProcessedDataMenu},
                { EnumViewStates.BackAction, null},
                { EnumViewStates.Exit, ExitMenu}
            };

            CommandLabels = new Dictionary<EnumViewStates, string>()
            {
                { EnumViewStates.Main, "Main" },
                { EnumViewStates.DeviceDefinition, "Define device" },
                { EnumViewStates.LoadDiscoveryData, "Load existing SNMP data discovery" },
                { EnumViewStates.ProcessSelection, "Select processing functions" },
                { EnumViewStates.ProcessExecution, "Run processes" },
                { EnumViewStates.NotificationSetting, "Set notification level" },
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
                { EnumViewStates.ProcessSelection, new EnumViewStates[]{ EnumViewStates.ProcessSelection, EnumViewStates.NotificationSetting, EnumViewStates.BackAction } },
                { EnumViewStates.NotificationSetting, new EnumViewStates[]{ EnumViewStates.ProcessExecution, EnumViewStates.BackAction }  },
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

        private void ShowCommands()
        {
            Console.WriteLine("----- Available commands -----\n");

            for (int i = 0; i < StateMachine[StateHistory.Peek()].Length; i++)
            {
                Console.WriteLine($"{i} - {CommandLabels[StateMachine[StateHistory.Peek()][i]]}");
            }

            Console.WriteLine();
        }

        private void GetCommand()
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

            ShowCommands();
            GetCommand();
            StateHandlers[StateHistory.Peek()].Invoke();
        }

        private void DefineDevice()
        {
            string settingname, initialIP, finalIP, SNMPuser;

            Console.Write("Setting name: ");
            settingname = Console.ReadLine();
            Console.Write("Initial IP: ");
            initialIP = Console.ReadLine();
            Console.Write("Final IP: ");
            finalIP = Console.ReadLine();
            Console.Write("SNMP community user (V2): ");
            SNMPuser = Console.ReadLine();
            Console.WriteLine();

            _controller.DefineDevice(settingname, initialIP, finalIP, SNMPuser);
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
            //Posible acitons

            NextActionHandle();
        }

        private void RunProcessMenu()
        {
            //Posible acitons

            NextActionHandle();
        }

        private void SetNotificationMenu()
        {
            //Posible acitons

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
            else
            {
                ShowData((ISNMPProcessedValueDTO)data);
            }
        }

        private void ShowData(ISNMPDeviceDTO data)
        {
            Console.WriteLine($"Added SNMP device {data.TargetIP}.\n");
        }

        private void ShowData(ISNMPSettingDTO data)
        {
            Console.WriteLine($"Added SNMP setting {data.ID} with this definition:\n" +
                              $"\t-Initial IP: {data.InitialIP}\n" +
                              $"\t-Final IP: {data.FinalIP}\n" +
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
            //throw new NotImplementedException();
        }

        #endregion
    }
}
