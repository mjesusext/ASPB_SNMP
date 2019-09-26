using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Controller.Helpers
{
    public static class ControllerHelper
    {
        public static bool ValidateIPv4Format(string ip)
        {
            bool result = false;
            int netmask, maskpos;
            int[] IPcomponents;

            //Contains mask
            maskpos = ip.IndexOf('/');
            if(maskpos == -1)
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
                if(IPcomponents[i] < 0 || IPcomponents[i] > 255)
                {
                    return result;
                } 
            }

            result = true;

            return result;
        }
    }
}
