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
        
        private int CurrentState, PreviousState;

        private Action[] StateHandlers;
        private readonly string[] CommandLabels =
        {
            "Main",
            "Load existing SNMP data discovery",
            "Select processing functions",
            "Run processes",
            "Set notification level",
            "Prompt data",
            "Exit",
            "Back to previous menu"
        };
        private readonly int[][] StateMachine = 
        { 
            new int[] { 1, 2, 6 },
            new int[] { 2, 7 },
            new int[] { 2, 3, 7 },
            new int[] { 4, 7 },
            new int[] { 5, 7 },
            new int[] { 3, 6, 7 }
        };

        //Mock for redirecting console to file
        private FileStream ostrm;
        private StreamWriter writer;
        private TextWriter oldOut;

        public SNMPDiscoveryView(ISNMPModelDTO Model, ISNMPDiscoveryController Controller)
        {
            _controller = Controller;
            _observeableSubscription = Model.Subscribe(this);

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

        #region Private Methods

        private void Initialize()
        {
            CurrentState = 0;
            PreviousState = 0;
            StateHandlers = new Action[]
            {
                BasicHandle,
                LoadDataMenu,
                ProcessingMenu,
                RunProcessMenu,
                SetNotificationMenu,
                PromptDataMenu,
                ExitMenu
            };

            Console.WriteLine("Welcome to ASPB network documentation tool.\n");
            StateHandlers[CurrentState].Invoke();
        }

        private void ShowCommands()
        {
            Console.WriteLine("----- Available commands -----\n");

            for (int i = 0; i < StateMachine[CurrentState].Length; i++)
            {
                Console.WriteLine($"{i} - {CommandLabels[StateMachine[CurrentState][i]]}");
            }
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
                else if (GoingState >= StateMachine[CurrentState].Length)
                {
                    Console.WriteLine("ERROR: Selected option not available");
                }
            } while (wrongInput);

            Console.WriteLine();

            //Back to previous level
            if(StateMachine[CurrentState][GoingState] == 7)
            {
                //int NextStep = PreviousState;
                //PreviousState = CurrentState;
                //CurrentState = NextStep;
                PreviousState = CurrentState--;
                BasicHandle();
            }
            else
            {
                PreviousState = CurrentState;
                CurrentState = StateMachine[CurrentState][GoingState];
            }
        }

        private void BasicHandle()
        {
            //Next steps
            ShowCommands();
            GetCommand();
            StateHandlers[CurrentState].Invoke();
        }

        private void LoadDataMenu()
        {
            //Posible acitons

            //Next steps
            ShowCommands();
            GetCommand();
            StateHandlers[CurrentState].Invoke();
        }

        private void ProcessingMenu()
        {
            //Posible acitons

            //Next steps
            ShowCommands();
            GetCommand();
            StateHandlers[CurrentState].Invoke();
        }

        private void RunProcessMenu()
        {
            //Posible acitons

            //Next steps
            ShowCommands();
            GetCommand();
            StateHandlers[CurrentState].Invoke();
        }

        private void SetNotificationMenu()
        {
            //Posible acitons

            //Next steps
            ShowCommands();
            GetCommand();
            StateHandlers[CurrentState].Invoke();
        }

        private void PromptDataMenu()
        {
            //Posible acitons

            //Next steps
            ShowCommands();
            GetCommand();
            StateHandlers[CurrentState].Invoke();
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
