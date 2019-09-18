using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public interface ITrackedObjectContainer
    {
        IDictionary<Type, List<string>> ChangedObjects { get; set; }
    }
}
