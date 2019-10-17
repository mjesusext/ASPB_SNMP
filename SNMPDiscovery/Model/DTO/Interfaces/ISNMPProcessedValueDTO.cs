using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    //Wrappers are not included on change tracking. Only the editor of wrapper object triggers change tracking when properly informed
    public interface ISNMPProcessedValueDTO
    {
        Type DataType { get; set; }
        object Data { get; set; }
    }
}