using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DAO
{
    interface ISNMPDataDAO <TData, TKey>
    {
        bool Create(TData Data);
        TData Retrieve(TKey Key);
        bool Update(TData Data);
        bool Delete(TKey Key);
        IList<TData> BulkRetrieve();
        void BulkCreate(IList<TData> BulkData);
        bool BulkAvailable();
    }
}
