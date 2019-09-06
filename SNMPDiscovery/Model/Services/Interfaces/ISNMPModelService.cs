using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.Services
{
    public interface ISNMPModelService : IObservable<ISNMPModelDTO>
    {
        //void LoadAppSettings(string AppSettingFilePath);
        //void LoadSNMPSettings(string SNMPSettingFilePath);
        //void LoadSNMPData();
        void StartDiscovery();
        void RunProcesses();
        void SaveData();
        //void SaveSettings();
    }
}
