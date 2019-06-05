using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.EncryptConfig.WAppExample.Configuration
{
    public class Jwt
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SecretKey { get; set; }
    }
}
