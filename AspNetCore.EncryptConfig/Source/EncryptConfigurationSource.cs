using AspNetCore.EncryptConfig.Cipher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.EncryptConfig.Source
{
    public class EncryptConfigurationSource : JsonConfigurationSource
    {
        public IEncryptor Encryptor { get; set; }
        public string KeyPath { get; set; }
        public string EquivalentPath { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            this.EnsureDefaults(builder);
            var encryptor = Encryptor ?? new DefaultEncryptor();
            return new JsonEncryptConfigurationProvider(this, encryptor);
        }
    }
}
