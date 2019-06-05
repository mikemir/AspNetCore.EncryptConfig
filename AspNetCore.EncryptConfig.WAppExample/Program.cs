using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.EncryptConfig.WAppExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());

            config.AddEncryptConfigFile($"appsettings.{environmentName}.jcif", 
                                        $"config.k.{environmentName}.jcif", 
                                        $"config.{environmentName}.jcif");

            return WebHost.CreateDefaultBuilder(args)
                            .UseConfiguration(config.Build())
                            .UseStartup<Startup>();
        }
    }
}
