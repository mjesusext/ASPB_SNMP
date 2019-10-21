using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SNMPDiscovery.Model.DTO;

namespace SNMPDiscovery.Model.DAO
{
    public class SNMPSettingXMLDAO : ISNMPDataDAO<ISNMPDeviceSettingDTO, string>
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

        public void BulkCreate(IList<ISNMPDeviceSettingDTO> BulkData)
        {
            throw new NotImplementedException();
        }

        public IList<ISNMPDeviceSettingDTO> BulkRetrieve()
        {
            throw new NotImplementedException();
        }

        public bool Create(ISNMPDeviceSettingDTO Data)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string Key)
        {
            throw new NotImplementedException();
        }

        public ISNMPDeviceSettingDTO Retrieve(string Key)
        {
            throw new NotImplementedException();
        }

        public bool Update(ISNMPDeviceSettingDTO Data)
        {
            throw new NotImplementedException();
        }
    }
}
