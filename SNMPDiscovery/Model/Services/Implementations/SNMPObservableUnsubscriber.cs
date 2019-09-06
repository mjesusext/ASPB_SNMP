using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.Services
{
    public class SNMPObservableUnsubscriber<T> : IDisposable
    {
        private IList<IObserver<T>> _observers;
        private IObserver<T> _observer;

        public SNMPObservableUnsubscriber(IList<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if(_observer != null && _observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }
}
