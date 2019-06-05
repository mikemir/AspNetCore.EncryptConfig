using AspNetCore.EncryptConfig.Cipher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.EncryptConfig.Source
{
    public class JsonEncryptConfigurationProvider : JsonConfigurationProvider
    {
        private const string KEY_PREFIX = "K3y";

        private readonly EncryptConfigurationSource _source;
        private readonly IEncryptor _encryptor;

        public JsonEncryptConfigurationProvider(JsonConfigurationSource source, IEncryptor encryptor) 
            : base(source)
        {
            _source = (EncryptConfigurationSource)source;
            _encryptor = encryptor;
        }

        private Dictionary<string, string> GetEquivalents(string json, out string secondKey)
        {
            var equivalentsResult = new Dictionary<string, string>();
            var equivalentsProperties = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            secondKey = $"{KEY_PREFIX}:{string.Join("", equivalentsProperties.Select(item => item.Key))}";

            foreach (var item in equivalentsProperties)
            {
                var decrypt = _encryptor.Decrypt(item.Value, secondKey);
                equivalentsResult.Add(item.Key, decrypt);
            }

            return equivalentsResult;
        }

        private string GetJsonAppSettings(string appsettings, Dictionary<string, string> equivalents, string key)
        {
            var jsonObjects = JsonConvert.DeserializeObject<Dictionary<string, object>>(appsettings)
                                         .Where(item => item.Key.Contains("Config."));

            foreach (var item in jsonObjects)
            {
                var jsonProps = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Value.ToString());
                foreach (var prop in jsonProps)
                {
                    appsettings = appsettings.Replace(prop.Key, equivalents[prop.Key]);
                    appsettings = appsettings.Replace(prop.Value, _encryptor.Decrypt(prop.Value, key));
                }
            }

            return appsettings;
        }

        public override void Load(Stream stream)
        {
            MemoryStream result = null;

            var keyPath = _source.KeyPath;
            if (!File.Exists(keyPath))
                throw new FileNotFoundException("Archivo de llave no encontrado.", keyPath);

            var equivalentsPath = _source.EquivalentPath;
            if(!File.Exists(equivalentsPath))
                throw new FileNotFoundException("Archivo de equivalentes no encontrado.", equivalentsPath);            

            try
            {
                var decodeFirstKey = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(keyPath)));
                var firstKey = $"{KEY_PREFIX}:{decodeFirstKey}";

                var equivalentsFile = File.ReadAllText(equivalentsPath);
                var equivalentsDecrypt = _encryptor.Decrypt(equivalentsFile, firstKey);
                var equivalents = GetEquivalents(equivalentsDecrypt, out string secondKey);

                var appSettingFile = new StreamReader(stream).ReadToEnd();
                var appSettingDecrypt = _encryptor.Decrypt(appSettingFile, secondKey);
                var appSettings = GetJsonAppSettings(appSettingDecrypt, equivalents, firstKey);

                result = new MemoryStream(Encoding.UTF8.GetBytes(appSettings));
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Ocurrió un error al descifrar uno de los archivos.", ex);
            }

            base.Load(result);
        }
    }
}