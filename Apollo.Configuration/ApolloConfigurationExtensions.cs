using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;

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

            ApolloConfigurationManagerHelper.SetApolloOptions(repositoryFactory);

            var apolloBuilder = new ApolloConfigurationBuilder(builder, repositoryFactory);
            builder.Properties[typeof(ApolloConfigurationExtensions).FullName] = apolloBuilder;

            if (options is ApolloOptions ao && ao.Namespaces != null)
                foreach (var ns in ao.Namespaces) apolloBuilder.AddNamespace(ns);

            return apolloBuilder;
        }

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder)
        {
            if (!builder.Properties.TryGetValue(typeof(ApolloConfigurationExtensions).FullName, out var apolloBuilder))
            {
                throw new InvalidOperationException("Please invoke 'AddApollo(options)' init apollo at the beginning.");
            }
            return (ApolloConfigurationBuilder)apolloBuilder;

        }
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            return builder.AddNamespace(ConfigConsts.NamespaceApplication, null, format);
        }

        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            return builder.AddNamespace(@namespace, null, format);
        }


        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, string? sectionKey, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentNullException(nameof(@namespace));
            }
            if (!Enum.IsDefined(typeof(ConfigFileFormat), format))
            {
                throw new ArgumentException("The enum value is not defined.", nameof(format));
            }

            if (format != ConfigFileFormat.Properties)
            {
                @namespace += "." + format.GetString();
            }

            var configRepository = builder.ConfigRepositoryFactory.GetConfigRepository(@namespace);
            var previous = builder.Sources.FirstOrDefault(source =>
                source is ApolloConfigurationProvider apollo &&
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

                ApolloConfigurationManagerHelper.Manager.Registry.Register(@namespace, new DefaultConfigFactory(builder.ConfigRepositoryFactory));
            }

            return builder;
        }
    }
}
