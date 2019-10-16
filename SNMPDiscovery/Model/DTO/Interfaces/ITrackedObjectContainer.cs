using SNMPDiscovery.Model.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ITrackedObjectContainer
    {
        CustomPair<Type, object> ChangedObject { get; set; }
    }
}
