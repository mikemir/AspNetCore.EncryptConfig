using AspNetCore.EncryptConfig.Source;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.EncryptConfig
{
    public static class JsonEncryptConfigurationExtensions
    {
        public static IConfigurationBuilder AddEncryptConfigFile(this IConfigurationBuilder builder, string configPath, string keyPath, string equivalentPath)
        {
            return AddEncryptConfigFile(builder, provider: null, configPath: configPath, keyPath: keyPath, equivalentPath: equivalentPath, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddEncryptConfigFile(this IConfigurationBuilder builder, string configPath, string keyPath, string equivalentPath, bool optional)
        {
            return AddEncryptConfigFile(builder, provider: null, configPath: configPath, keyPath: keyPath, equivalentPath: equivalentPath, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddEncryptConfigFile(this IConfigurationBuilder builder, string configPath, string keyPath, string equivalentPath, bool optional, bool reloadOnChange)
        {
            return AddEncryptConfigFile(builder, provider: null, configPath: configPath, keyPath: keyPath, equivalentPath: equivalentPath, optional: optional, reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddEncryptConfigFile(this IConfigurationBuilder builder, IFileProvider provider, string configPath, string keyPath, string equivalentPath, bool optional, bool reloadOnChange)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            if (string.IsNullOrEmpty(configPath))
                throw new ArgumentNullException(nameof(configPath));
            

            return builder.AddEncryptConfigFile(source =>
            {
                source.FileProvider = provider;
                source.Path = configPath;
                source.Optional = optional;
                source.ReloadOnChange = reloadOnChange;
                source.KeyPath = keyPath;
                source.EquivalentPath = equivalentPath;
                source.ResolveFileProvider();
            });
        }

        public static IConfigurationBuilder AddEncryptConfigFile(this IConfigurationBuilder builder, Action<EncryptConfigurationSource> configureSource) => builder.Add(configureSource);
    }
}
