using EncryptConfig.CApp.Cipher;
using EncryptConfig.CApp.Stenography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptConfig.CApp
{
    class Program
    {
        private const string KEY_PREFIX = "K3y";

        private static void WriteLine(string text, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static string ObfuscateAppsettings(string appsettings, string firstKey, out Dictionary<string, string> equivalents)
        {

            equivalents = new Dictionary<string, string>();
            var jsonObjects = JsonConvert.DeserializeObject<Dictionary<string, object>>(appsettings)
                                         .Where(item => item.Key.Contains("Config."));
            
            foreach (var item in jsonObjects)
            {
                var jsonProps = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Value.ToString());
                foreach (var prop in jsonProps)
                {
                    var obfuscateText = prop.Key.Obfuscate();
                    equivalents.Add(prop.Key, obfuscateText);
                    appsettings = appsettings.Replace(prop.Key, obfuscateText);
                    appsettings = appsettings.Replace(prop.Value, Encryptor.Encrypt(prop.Value, firstKey));
                }
            }

            return appsettings;
        }

        private static string ParseEquivalentsToJson(Dictionary<string, string> equivalents, string firstKey)
        {
            var jsonObject = new JObject();
            foreach (var item in equivalents)
            {
                var encryptKey = Encryptor.Encrypt(item.Key, firstKey);
                jsonObject.Add(item.Value, JToken.FromObject(encryptKey));
            }

            return jsonObject.ToString();
        }

        static void Main(string[] args)
        {
            var print = args.Contains("-v");

            try
            {
                var env = "Development";
                var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var keyPath = Path.Combine(directory, $"config.k.{env}.enc");
                var appsettingsPath = Path.Combine(directory, $"appsettings.{env}.enc");

                //0.0 obtenemos la primer llave en el archivo
                var firstKey = $"{KEY_PREFIX}:{File.ReadAllText(keyPath)}"; //Ojo: se agrega la palabra "K3y" a la llave principal
                var encodeKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(firstKey.Replace("K3y:", "")));

                //1.0 encriptamos las propiedades del archivo appsettings con la llave principal
                var appsettings = File.ReadAllText(appsettingsPath);
                if (print) WriteLine($"Load Settings => {appsettings}");
                var obfusAppsettings = ObfuscateAppsettings(appsettings, firstKey, out Dictionary<string, string> equivalentsProperties);
                if (print)
                {
                    WriteLine($"Obfuscate => {obfusAppsettings}", ConsoleColor.DarkCyan);
                    WriteLine($"Load Equivalents => \n{string.Join("\n", equivalentsProperties.Select(item => $"{item.Value}: {item.Key}"))}");
                }

                //2.0 obtenemos la llave secundaria del archivo de equivalentes
                var secondKey = $"{KEY_PREFIX}:{string.Join("", equivalentsProperties.Select(item => item.Value))}";

                //3.0 encriptamos el archivo de equivalentes con la llave principal pero el contenido de las propiedades con la llave secundaria
                var equivalents = ParseEquivalentsToJson(equivalentsProperties, secondKey);
                var encryptInfo = Encryptor.Encrypt(equivalents, firstKey);
                if (print) WriteLine($"Equivalents => {equivalents}", ConsoleColor.DarkYellow);

                //4.0 encriptamos el archivo appsettings con la llave secundaria
                var encryptAppsettings = Encryptor.Encrypt(obfusAppsettings, secondKey);

                //5.0 creación de nuevos archivos
                var dir = Directory.CreateDirectory(Path.Combine(directory, "cipher"));
                File.WriteAllText(Path.Combine(dir.FullName, $"config.{env}.jcif"), encryptInfo);            //5.1 guardamos archivo de equivalentes
                File.WriteAllText(Path.Combine(dir.FullName, $"appsettings.{env}.jcif"), encryptAppsettings);//5.2 guardamos archivo de appsettings
                File.WriteAllText(Path.Combine(dir.FullName, $"config.k.{env}.jcif"), encodeKey);            //5.3 guardado archivo de llave principal codificada base64

                WriteLine("Proceso finalizado con éxito...", ConsoleColor.DarkGreen);
            }
            catch (Exception ex)
            {
                WriteLine($"Error: {ex.Message}", ConsoleColor.DarkRed);
            }

            WriteLine("Presion <Enter> para salir.");
            Console.Read();
        }
    }
}
