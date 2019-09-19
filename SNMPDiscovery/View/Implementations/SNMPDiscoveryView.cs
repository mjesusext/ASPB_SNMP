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
    public class SNMPDiscoveryView : ISNMPView
    {
        private ISNMPDiscoveryController _controller { get; set; }
        private IDisposable _observeableSubscription { get; set; }
        
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

        #region Observable Implementation

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

        #region View Implementation

        public void Initialize()
        {
            Console.WriteLine("Inicializando herramienta de documentación de red.\n");
            Console.ReadKey();
        }

        public void ShowCommands()
        {
            throw new NotImplementedException();
        }

        public void ShowData(ISNMPDeviceDTO data)
        {
            Console.WriteLine($"Added SNMP device {data.TargetIP}.\n");
        }

        public void ShowData(ISNMPSettingDTO data)
        {
            Console.WriteLine($"Added SNMP setting {data.ID} with this definition:\n" +
                              $"\t-Initial IP: {data.InitialIP}\n" +
                              $"\t-Final IP: {data.FinalIP}\n" +
                              $"\t-Community string: {data.CommunityString}\n");

        }

        public void ShowData(ISNMPProcessStrategy data)
        {
            Console.WriteLine($"Added process setting {data.ProcessID} related to {data.RegardingSetting}.\n");
        }

        public void ShowData(IOIDSettingDTO data)
        {
            Console.WriteLine($"Added OID setting {data.ID} with this definition:\n" +
                              $"\t-Initial OID: {data.InitialOID}\n" +
                              $"\t-Final OID: {data.FinalOID}\n" +
                              $"\t-Inclusive: {data.InclusiveInterval}\n");
        }

        public void ShowData(ISNMPRawEntryDTO data)
        {
            Console.WriteLine($"Added OID data. Identifier: {data.OID}. DataType: {data.DataType}. Value: {data.ValueData}.\n");
        }

        public void ShowData(ISNMPProcessedValueDTO data)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Private methods

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

        #endregion
    }
}
