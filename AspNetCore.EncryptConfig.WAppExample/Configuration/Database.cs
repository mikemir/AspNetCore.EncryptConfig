using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.EncryptConfig.WAppExample.Configuration
{
    public class Database
    {
        public string ConnectionString { get; set; }

        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
