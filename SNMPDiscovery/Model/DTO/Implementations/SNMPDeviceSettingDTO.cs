using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SNMPDiscovery.Model.Helpers;
using SNMPDiscovery.Model.Services;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPDeviceSettingDTO : ISNMPDeviceSettingDTO
    {
        public string ID { get; set; }
        public ISNMPModelDTO RegardingObject { get; set; }

        public IPAddress InitialIP { get; set; }
        public IPAddress FinalIP { get; set; }
        public int NetworkMask { get; set; }
        public string CommunityString { get; set; }
        public event Action<object, Type> OnChange;

        #region Nested Object Change Handlers

        public void ChangeTrackerHandler(object obj, Type type)
        {
            OnChange?.Invoke(obj, type);
        }

        #endregion

        #region Constructors

        public SNMPDeviceSettingDTO(string id, string initialIPAndMask, string finalIPAndMask, string SNMPUser, Action<object, Type> ChangeTrackerHandler)
        {
            ID = id;
            InitialIP = ModelHelper.ExtractIPAddress(initialIPAndMask);
            FinalIP = finalIPAndMask == null ? InitialIP : ModelHelper.ExtractIPAddress(finalIPAndMask);
            NetworkMask = ModelHelper.ExtractNetworkMask(initialIPAndMask);
            CommunityString = SNMPUser;

            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPDeviceSettingDTO));
        }

        #endregion

        #region Data editors

        public void EditDeviceSetting(string newid, string initialIPAndMask, string finalIPAndMask, string SNMPUser)
        {
            ID = newid;
            InitialIP = ModelHelper.ExtractIPAddress(initialIPAndMask);
            FinalIP = finalIPAndMask == null ? InitialIP : ModelHelper.ExtractIPAddress(finalIPAndMask);
            NetworkMask = ModelHelper.ExtractNetworkMask(initialIPAndMask);
            CommunityString = SNMPUser;

            OnChange?.Invoke(this, typeof(ISNMPDeviceSettingDTO));
        }

        #endregion
    }
}