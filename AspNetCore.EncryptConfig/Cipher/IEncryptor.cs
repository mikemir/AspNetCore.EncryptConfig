using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.EncryptConfig.Cipher
{
    public interface IEncryptor
    {
        string Encrypt(string data, string key);

        string Decrypt(string data, string key);
    }
}
