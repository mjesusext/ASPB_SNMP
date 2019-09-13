using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.Helpers
{
    public class CustomPair<Tfirst, Tsecond>
    {
        public Tfirst First { get; set; }
        public Tsecond Second { get; set; }

        public CustomPair(Tfirst first, Tsecond second)
        {
            First = first;
            Second = second;
        }
            
    }
}
