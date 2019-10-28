using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DAO
{
    interface ISNMPDataDAO <TData, TKey>
    {
        int Create(TData Data);
        TData Retrieve(TKey Key);
        int Update(TData Data);
        int Delete(TKey Key);
        IList<TData> BulkRetrieve();
        int BulkCreate(IList<TData> BulkData);
        bool BulkAvailable();
    }
}
