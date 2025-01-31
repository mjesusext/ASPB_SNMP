﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.Helpers
{
    public static class ModelHelper
    {
        #region IP Computations

        public static bool ValidateIPAndMask(string IPAndMask)
        {
            bool result = false;
            int netmask, maskpos;
            int[] IPcomponents;

            //Contains mask
            maskpos = IPAndMask.IndexOf('/');
            if (maskpos == -1)
            {
                return result;
            }

            //Mask range and conversion
            result = int.TryParse(IPAndMask.Substring(maskpos + 1), out netmask);
            if (!result && (netmask < 0 || netmask > 32))
            {
                return result;
            }

            //Address conversion
            try
            {
                IPcomponents = IPAndMask.Substring(0, maskpos - 1).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            }
            catch
            {
                return result;
            }

            //Check # of sections = 4
            if (IPcomponents.Length != 4)
            {
                return result;
            }

            //Check 0 to 255 range of every component
            for (int i = 0; i < IPcomponents.Length; i++)
            {
                if (IPcomponents[i] < 0 || IPcomponents[i] > 255)
                {
                    return result;
                }
            }

            result = true;

            return result;
        }

        public static bool ValidateIPandMaskRange(string initialIPAndMask, string finalIPAndMask)
        {
            bool result = false;
            int initialIPAddress, finalIPAddress;
            int initialMask, finalMask, maskpos;

            if (string.IsNullOrWhiteSpace(initialIPAndMask) || string.IsNullOrWhiteSpace(finalIPAndMask))
            {
                return result;
            }

            //Decompose inputs
            maskpos = initialIPAndMask.IndexOf('/');
            //initialIPAddress = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(initialIPAndMask.Substring(0, maskpos - 1)).GetAddressBytes(), 0));
            initialIPAddress = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(initialIPAndMask.Substring(0, maskpos)).GetAddressBytes(), 0));
            initialMask = int.Parse(initialIPAndMask.Substring(maskpos + 1));

            maskpos = finalIPAndMask.IndexOf('/');
            //finalIPAddress = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(finalIPAndMask.Substring(0, maskpos - 1)).GetAddressBytes(), 0));
            finalIPAddress = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(finalIPAndMask.Substring(0, maskpos)).GetAddressBytes(), 0));
            finalMask = int.Parse(finalIPAndMask.Substring(maskpos + 1));

            //Previous validation
            if (initialIPAddress <= finalIPAddress && finalMask == initialMask)
            {
                result = true;
            }

            return result;
        }

        public static IPAddress ExtractIPAddress(string targetIPAndMask)
        {
            int maskpos;

            maskpos = targetIPAndMask.IndexOf('/');
            return IPAddress.Parse(targetIPAndMask.Substring(0, maskpos));
        }

        public static int ExtractNetworkMask(string targetIPAndMask)
        {
            int maskpos;

            maskpos = targetIPAndMask.IndexOf('/');
            return int.Parse(targetIPAndMask.Substring(maskpos + 1));
        }

        public static IList<IPAddress> GenerateHostList(IPAddress initialIP, IPAddress finalIP, int NetworkMask, List<string> LeasedIPs = null)
        {
            IList<IPAddress> res = new List<IPAddress>();
            IPAddress netMask;

            //Set variables for functionality
            int LowerIPboundSNMP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(initialIP.GetAddressBytes(), 0));
            int UpperIPboundSNMP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(finalIP.GetAddressBytes(), 0));
            netMask = CreateMaskByNetBitLength(NetworkMask);

            for (int i = LowerIPboundSNMP; i <= UpperIPboundSNMP; i++)
            {
                IPAddress currentIP = new IPAddress(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(i)));
                IPAddress broadcastIP = GetBroadcastAddress(currentIP, netMask);
                IPAddress networkIP = GetNetworkAddress(currentIP, netMask);

                if (!currentIP.Equals(broadcastIP) && !currentIP.Equals(networkIP))
                {
                    //MJE 130321 - Añade elementos solo si son IPs concedidas por el DHCP. Anulamos validación de momento.

                    //Compare with list of IP currently being used or generate full range
                    //if (LeasedIPs == null || LeasedIPs.Contains(currentIP.ToString()))
                    //{
                        res.Add(currentIP);
                    //}
                }
            }

            return res;
        }
        
        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
            {
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");
            }

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
            {
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");
            }

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool IsInSameSubnet(IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            IPAddress network1 = GetNetworkAddress(address, subnetMask);
            IPAddress network2 = GetNetworkAddress(address2, subnetMask);

            return network1.Equals(network2);
        }

        public static IPAddress CreateMaskByHostBitLength(int hostpartLength)
        {
            int hostPartLength = hostpartLength;
            int netPartLength = 32 - hostPartLength;

            if (netPartLength < 2)
                throw new ArgumentException("Number of hosts is to large for IPv4");

            Byte[] binaryMask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                if (i * 8 + 8 <= netPartLength)
                {
                    binaryMask[i] = (byte)255;
                }
                else if (i * 8 > netPartLength)
                {
                    binaryMask[i] = (byte)0;
                }
                else
                {
                    int oneLength = netPartLength - i * 8;
                    string binaryDigit = string.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
                    binaryMask[i] = Convert.ToByte(binaryDigit, 2);
                }
            }
            return new IPAddress(binaryMask);
        }

        public static IPAddress CreateMaskByNetBitLength(int netpartLength)
        {
            int hostPartLength = 32 - netpartLength;
            return CreateMaskByHostBitLength(hostPartLength);
        }

        public static IPAddress CreateMaskByHostNumber(int numberOfHosts)
        {
            int maxNumber = numberOfHosts + 1;

            string b = Convert.ToString(maxNumber, 2);

            return CreateMaskByHostBitLength(b.Length);
        }

        #endregion

        #region ARP - RARP

        [DllImport("iphlpapi.dll", ExactSpelling = true, EntryPoint = "SendARP")]
        private static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint DhcpEnumSubnetClients(string ServerIpAddress, uint SubnetAddress, ref uint ResumeHandle, uint PreferredMaximum, out IntPtr ClientInfo, ref uint ElementsRead, ref uint ElementsTotal        );

        #region Structs DCHP API code

        [StructLayout(LayoutKind.Sequential)]
        public struct DHCP_CLIENT_INFO_ARRAY
        {
            public uint NumElements;
            public IntPtr Clients;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DHCP_CLIENT_UID
        {
            public uint DataLength;
            public IntPtr Data;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DHCP_CLIENT_INFO
        {
            public uint ip;
            public uint subnet;

            public DHCP_CLIENT_UID mac;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClientName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClientComment;
        }

        #endregion

        public static string SendARPRequest(string IPaddress)
        {
            int MAClenght = 6;
            byte[] MACAddress = new byte[MAClenght];
            //Host order is assumed
            int formattedtip = BitConverter.ToInt32(IPAddress.Parse(IPaddress).GetAddressBytes(), 0);

            if (SendARP(formattedtip, 0, MACAddress, ref MAClenght) == 0)
            {
                return string.Join(" ", MACAddress.Select(x => string.Format("{0:x2}", x).ToUpper()));
            }
            else
            {
                return null;
            }
        }

        public static void GetDHCPleases(List<string> DHCPServerIPs, IDictionary<string, string> MACIPmapping)
        {
            uint parsedMask = 0; //Gets all scopes
            uint resumeHandle = 0;
            uint numClientsRead = 0;
            uint totalClients = 0;
            IntPtr info_array_ptr = IntPtr.Zero;

            foreach (string DHCPServer in DHCPServerIPs)
            {
                uint response = DhcpEnumSubnetClients(DHCPServer, parsedMask, ref resumeHandle, 65536, out info_array_ptr, ref numClientsRead, ref totalClients);

                //If no results, next DHCP server
                if((int)info_array_ptr == 0)
                {
                    continue;
                }

                //Set up client array casted to a DHCP_CLIENT_INFO_ARRAY using the pointer from the response object above
                DHCP_CLIENT_INFO_ARRAY rawClients = (DHCP_CLIENT_INFO_ARRAY)Marshal.PtrToStructure(info_array_ptr, typeof(DHCP_CLIENT_INFO_ARRAY));

                // Loop through the clients structure inside rawClients adding to the dchpClient collection
                IntPtr current = rawClients.Clients;

                for (int i = 0; i < (int)rawClients.NumElements; i++)
                {
                    string machineIP, machineMAC;
                    
                    //Create machine object using the struct
                    DHCP_CLIENT_INFO rawMachine = (DHCP_CLIENT_INFO)Marshal.PtrToStructure(Marshal.ReadIntPtr(current), typeof(DHCP_CLIENT_INFO));

                    var a = BitConverter.GetBytes(rawMachine.ip).Reverse().ToArray();
                    var b = BitConverter.ToUInt32(a, 0);

                    //Transform machine information
                    machineIP = new IPAddress(b).ToString();

                    machineMAC = string.Format("{0:x2} {1:x2} {2:x2} {3:x2} {4:x2} {5:x2}",
                        Marshal.ReadByte(rawMachine.mac.Data),
                        Marshal.ReadByte(rawMachine.mac.Data, 1),
                        Marshal.ReadByte(rawMachine.mac.Data, 2),
                        Marshal.ReadByte(rawMachine.mac.Data, 3),
                        Marshal.ReadByte(rawMachine.mac.Data, 4),
                        Marshal.ReadByte(rawMachine.mac.Data, 5)).ToUpper();

                    //Add to mapping object
                    MACIPmapping.Add(machineMAC, machineIP);

                    //Move pointer to next machine
                    current = (IntPtr)((int)current + (int)Marshal.SizeOf(typeof(IntPtr)));
                }
            }
        }

        #endregion
    }
}


