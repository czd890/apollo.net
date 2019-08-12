using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class ApolloConfigurationExtensions
    {
        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, IConfiguration apolloConfiguration)
        {
            return builder.AddApollo(apolloConfiguration.Get<ApolloOptions>());
        }

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, string appId, string metaServer)
        {
            return builder.AddApollo(new ApolloOptions { AppId = appId, MetaServer = metaServer });
        }

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, IApolloOptions options)
        {
            if (builder.Properties.ContainsKey(typeof(ApolloConfigurationExtensions).FullName))
            {
                throw new InvalidOperationException("Do not repeat init apollo");
            }
            var repositoryFactory = new ConfigRepositoryFactory(options ?? throw new ArgumentNullException(nameof(options)));
#pragma warning disable 618
            ApolloConfigurationManager.SetApolloOptions(repositoryFactory);
#pragma warning restore 618
            var apolloBuilder = new ApolloConfigurationBuilder(builder, repositoryFactory);
            builder.Properties[typeof(ApolloConfigurationExtensions).FullName] = apolloBuilder;
            return apolloBuilder;
        }

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder)
        {
            if (!builder.Properties.ContainsKey(typeof(ApolloConfigurationExtensions).FullName))
            {
                throw new InvalidOperationException("Please invoke 'AddApollo(options)' init apollo at the beginning.");
            }

            return builder.Properties[typeof(ApolloConfigurationExtensions).FullName] as ApolloConfigurationBuilder;

        }
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        /// <summary>添加默认namespace: application，等价于AddNamespace(ConfigConsts.NamespaceApplication)</summary>
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            return builder.AddNamespace(ConfigConsts.NamespaceApplication, null, format);
        }

        /// <summary>添加其他namespace</summary>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            return builder.AddNamespace(@namespace, null, format);
        }

        /// <summary>添加其他namespace。如果sectionKey为null则添加到root中，可以直接读取，否则使用Configuration.GetSection(sectionKey)读取</summary>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, string sectionKey, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            if (format < ConfigFileFormat.Properties || format > ConfigFileFormat.Txt)
            {
                throw new ArgumentOutOfRangeException(nameof(format), format, $"minValue:{ConfigFileFormat.Properties}，maxValue:{ConfigFileFormat.Txt}");
            }

            if (format != ConfigFileFormat.Properties)
            {
                @namespace += "." + format.ToString().ToLower();
            }

            var configRepository = builder.ConfigRepositoryFactory.GetConfigRepository(@namespace);
            var previous = builder.Sources.FirstOrDefault(source => source is ApolloConfigurationProvider apollo &&
                                                        apollo.SectionKey == sectionKey &&
                                                        apollo.ConfigRepository == configRepository);
            if (previous != null)
            {
                builder.Sources.Remove(previous);
                builder.Sources.Add(previous);
            }
            else
            {
                builder.Add(new ApolloConfigurationProvider(sectionKey, configRepository));
#pragma warning disable 618
                ApolloConfigurationManager.Manager.Registry.Register(@namespace, new DefaultConfigFactory(builder.ConfigRepositoryFactory));
#pragma warning restore 618
            }

            return builder;
        }
    }
}
