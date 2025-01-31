﻿using SNMPDiscovery.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface IDeviceTopologyInfoDTO : IDiscoveredBasicInfo
    {
        //Key: Port ID, Value: Port Description (text id)
        IDictionary<string, string> PortDescriptions { get; set; }
        //Key: Port ID, Value: Tuple of type and equivalence if needed (for trunks)
        IDictionary<string, CustomPair<EnumPhysPortType, string>> PortSettings { get; set; }
        //Key: Port ID, Value: MAC Address
        IDictionary<string, string> PortMACAddress { get; set; }
        //Key: VLAN ID, Value: VLAN name
        IDictionary<string, string> VLANInventory { get; set; }
        //Key: Port ID, Value: VLAN ID attached
        IDictionary<string, List<string>> PortVLANMapping { get; set; }
        //Key: Port ID, Value: Dictionary of MAC Address - IP Address
        IDictionary<string, IDictionary<string,string>> PortLearnedAddresses { get; set; }
        //Key: Port ID (Aggregate - Inferred trunk) , Value: Tuple of destination SWITCH CPU MAC - Port index of destination SWITCH
        IDictionary<string, CustomPair<string, string>> PortAggregateDestinations { get; set; }
        //Key: Port ID, Value: Tuple of MAC Address - IP Address
        IDictionary<string, IDictionary<string, string>> DeviceDirectNeighbours { get; set; }
    }
}
