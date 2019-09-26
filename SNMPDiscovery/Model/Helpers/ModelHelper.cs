﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.Helpers
{
    public static class ModelHelper
    {
        public static bool ValidateIPAndMask(string ip)
        {
            bool result = false;
            int netmask, maskpos;
            int[] IPcomponents;

            //Contains mask
            maskpos = ip.IndexOf('/');
            if (maskpos == -1)
            {
                return result;
            }

            //Mask range and conversion
            result = int.TryParse(ip.Substring(maskpos + 1), out netmask);
            if (!result && (netmask < 0 || netmask > 32))
            {
                return result;
            }

            //Address conversion
            try
            {
                IPcomponents = ip.Substring(0, maskpos - 1).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
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

        public static IList<IPAddress> GenerateHostList(string initialIPandMask, string finalIPandMask)
        {
            IList<IPAddress> res = new List<IPAddress>();

            string initialAddressComponent, finalAddressComponent;
            int initialMaskComponent, finalMaskComponent, maskpos;

            //Decompose inputs
            maskpos = initialIPandMask.IndexOf('/');
            initialAddressComponent = initialIPandMask.Substring(0, maskpos - 1);
            initialMaskComponent = int.Parse(initialIPandMask.Substring(maskpos + 1));

            maskpos = finalIPandMask.IndexOf('/');
            finalAddressComponent = finalIPandMask.Substring(0, maskpos - 1);
            finalMaskComponent = int.Parse(finalIPandMask.Substring(maskpos + 1));

            //Get variable values of IP
            int LowerIPboundSNMP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(initialAddressComponent).GetAddressBytes(), 0));
            int UpperIPboundSNMP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(finalAddressComponent).GetAddressBytes(), 0));

            for (int i = LowerIPboundSNMP; i <= UpperIPboundSNMP; i++)
            {
                IPAddress currentIP = new IPAddress(i);
                IPAddress broadcastIP = GetBroadcastAddress(currentIP, CreateMaskByNetBitLength(initialMaskComponent));
                IPAddress networkIP = GetNetworkAddress(currentIP, CreateMaskByNetBitLength(initialMaskComponent));

                if(!currentIP.Equals(broadcastIP) && !currentIP.Equals(networkIP))
                {
                    res.Add(currentIP);
                }
            }

            return res;
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

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
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

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
                    binaryMask[i] = (byte)255;
                else if (i * 8 > netPartLength)
                    binaryMask[i] = (byte)0;
                else
                {
                    int oneLength = netPartLength - i * 8;
                    string binaryDigit =
                        String.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
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
    }
}
