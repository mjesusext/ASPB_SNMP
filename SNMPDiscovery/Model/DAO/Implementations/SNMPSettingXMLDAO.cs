using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.DAO
{
    public class SNMPSettingXMLDAO : ISNMPDataDAO<ISNMPSettingDTO, string>
    {
        //private bool _bulk;
        //Escritor XML
        //Stream

        public SNMPSettingXMLDAO()
        {
            //_bulk = true;
        }

        public bool BulkAvailable()
        {
            throw new NotImplementedException();
        }

        public void BulkCreate(IList<ISNMPSettingDTO> BulkData)
        {
            throw new NotImplementedException();
        }

        public IList<ISNMPSettingDTO> BulkRetrieve()
        {
            throw new NotImplementedException();
        }

        public bool Create(ISNMPSettingDTO Data)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string Key)
        {
            throw new NotImplementedException();
        }

        public ISNMPSettingDTO Retrieve(string Key)
        {
            throw new NotImplementedException();
        }

        public bool Update(ISNMPSettingDTO Data)
        {
            throw new NotImplementedException();
        }
    }
}
