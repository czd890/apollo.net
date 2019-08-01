using Com.Ctrip.Framework.Apollo.Internals;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo
{
    public interface IApolloConfigurationBuilder : IConfigurationBuilder
    {
        ConfigRepositoryFactory ConfigRepositoryFactory { get; }
    }

    internal class ApolloConfigurationBuilder : IApolloConfigurationBuilder
    {
        private readonly IConfigurationBuilder _builder;

        public ConfigRepositoryFactory ConfigRepositoryFactory { get; }

        public ApolloConfigurationBuilder(IConfigurationBuilder builder, ConfigRepositoryFactory configRepositoryFactory)
            : this(builder)
        {
            ConfigRepositoryFactory = configRepositoryFactory;
        }
        public ApolloConfigurationBuilder(IConfigurationBuilder builder)
        {
            this._builder = builder;
        }

        public IConfigurationBuilder Add(IConfigurationSource source)
        {
            return _builder.Add(source);
        }

        public IConfigurationRoot Build()
        {
            return _builder.Build();
        }

        public IDictionary<string, object> Properties => _builder.Properties;
        public IList<IConfigurationSource> Sources => _builder.Sources;
    }
}
