using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPModel : ISNMPModelDTO
    {
        public IDictionary<string, ISNMPSettingDTO> SNMPSettings { get; set; }
        public IDictionary<string, ISNMPDeviceDTO> SNMPData { get; set; }

        public SNMPModel()
        {
        }

        #region Builders


        public ISNMPSettingDTO BuildSNMPSetting(string ID, string initialIP, string finalIP, string SNMPUser)
        {
            //Lazy initialization
            if (SNMPSettings == null)
            {
                SNMPSettings = new Dictionary<string, ISNMPSettingDTO>();
            }

            ISNMPSettingDTO setting = new SNMPSettingDTO(ID, initialIP, finalIP, SNMPUser);
            SNMPSettings.Add(ID, setting);

            return setting;
        }

        public ISNMPDeviceDTO BuildSNMPDevice(string targetIP)
        {
            //Lazy initialization
            if (SNMPData == null)
            {
                SNMPData = new Dictionary<string, ISNMPDeviceDTO>();
            }

            ISNMPDeviceDTO device = new SNMPDeviceDTO(targetIP);
            SNMPData.Add(targetIP, device);

            return device;
        }

        public ISNMPDeviceDTO BuildSNMPDevice(int targetIP)
        {
            //Lazy initialization
            if (SNMPData == null)
            {
                SNMPData = new Dictionary<string, ISNMPDeviceDTO>();
            }

            ISNMPDeviceDTO device = new SNMPDeviceDTO(targetIP);
            SNMPData.Add(new IPAddress(targetIP).ToString(), device);

            return device;
        }

        #endregion


    }
}
