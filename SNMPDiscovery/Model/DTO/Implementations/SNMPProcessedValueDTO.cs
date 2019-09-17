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
        private IList<IObserver<ISNMPProcessedValueDTO>> _snmpProcessedValueObservers;

        public Type DataType { get; set; }
        public object Data { get; set; }

        #region Interface Implementations

        public IDisposable Subscribe(IObserver<ISNMPProcessedValueDTO> observer)
        {
            //Check whether observer is already registered. If not, add it
            if (!_snmpProcessedValueObservers.Contains(observer))
            {
                _snmpProcessedValueObservers.Add(observer);
            }
            return new SNMPObservableUnsubscriber<ISNMPProcessedValueDTO>(_snmpProcessedValueObservers, observer);
        }

        #endregion

        #region Constructors

        public SNMPProcessedValueDTO(Type dataType, object data)
        {
            _snmpProcessedValueObservers = new List<IObserver<ISNMPProcessedValueDTO>>();
            DataType = dataType;
            Data = data;
        }

        #endregion
    }
}
