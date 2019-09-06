﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.Model.DTO
{
    public class SNMPRawEntryDTO : ISNMPRawEntryDTO
    {
        public string OID { get; set; }
        public string RootOID { get; set; }
        public string ValueData { get; set; }
        public EnumSNMPOIDType DataType { get; set; }

        public SNMPRawEntryDTO()
        {
        }

        public SNMPRawEntryDTO(string oid, string oidbase)
        {
            OID = oid;
            RootOID = oidbase;
        }

        public SNMPRawEntryDTO(string oid, string oidbase, string data, EnumSNMPOIDType datatype)
        {
            OID = oid;
            RootOID = oidbase;
            ValueData = data;
            DataType = datatype;
        }
    }
}
