using SNMPDiscovery.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPProcessedValueDTO : ISNMPProcessedValueDTO
    {
        public Type DataType { get; set; }
        public object Data { get; set; }

        #region Constructors

        public SNMPProcessedValueDTO(Type dataType, object data)
        {
            DataType = dataType;
            Data = data;
        }

        #endregion
    }
}
