using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ISNMPProcessedValueDTO
    {
        Type DataType { get; set; }
        object Data { get; set; }
    }
}