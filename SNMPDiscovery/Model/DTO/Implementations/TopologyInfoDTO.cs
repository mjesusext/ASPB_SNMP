using SNMPDiscovery.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class TopologyInfoDTO : IDeviceTopologyInfoDTO
    {
        public string DeviceIPAndMask { get; set; }
        public string DeviceMAC { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string OIDobjectID { get; set; }
        public EnumOSILayers OSIImplementedLayers { get; set; }
        public EnumDeviceType DeviceType { get; set; }

        public IDictionary<string, string> PortInventory { get; set; }
        public IDictionary<string, CustomPair<EnumPhysPortType, string>> PortSettings { get; set; }
        public IDictionary<string, string> PortMACAddress { get; set; }
        public IDictionary<string, string> VLANInventory { get; set; }
        public IDictionary<string, List<string>> PortVLANMapping { get; set; }
        public IDictionary<string, IDictionary<string, string>> PortLearnedAddresses { get; set; }
        public IDictionary<string, CustomPair<string, string>> PortAggregateDestinations { get; set; }
        public IDictionary<string, IDictionary<string, string>> DeviceDirectNeighbours { get; set; }

    }
}
