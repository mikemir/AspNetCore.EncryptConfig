using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptConfig.CApp.Stenography
{
    public static class HideExtensions
    {
        public static string Obfuscate(this string texto)
        {
            var result = new StringBuilder();
            var random = new Random(DateTime.Now.Millisecond + texto.Length);
            var cantidad = random.Next(10, 20);
            var source = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789$%!";

            for (int i = 0; i < cantidad; i++)
            {
                var index = random.Next(source.Length -1);
                result.Append(source[index]);
            }

            return result.ToString();
        }
    }
}
