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
        public event Action<object, Type> OnChange;

        #region Constructors

        public SNMPProcessedValueDTO(Type dataType, object data, Action<object, Type> ChangeTrackerHandler)
        {
            DataType = dataType;
            Data = data;
            OnChange += ChangeTrackerHandler;

            OnChange?.Invoke(this, typeof(ISNMPProcessedValueDTO));
        }

        #endregion
    }
}
