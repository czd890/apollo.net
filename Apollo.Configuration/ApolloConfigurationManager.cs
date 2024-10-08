﻿using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;

namespace Com.Ctrip.Framework.Apollo;

/// <summary>
/// Entry point for client config use
/// </summary>
[Obsolete("Not recommended for use，Recommended use Microsoft.Extensions.Configuration.IConfiguration")]
public class ApolloConfigurationManager
{
    public static IConfigManager Manager => ApolloConfigurationManagerHelper.Manager;

    /// <summary>
    /// Get Application's config instance. </summary>
    /// <returns> config instance </returns>
    public Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

    /// <summary>
    /// Get the config instance for the namespace. </summary>
    /// <param name="namespaceName"> the namespace of the config </param>
    /// <returns> config instance </returns>
    public Task<IConfig> GetConfig(string namespaceName)
    {
        if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));

        return Manager.GetConfig(namespaceName);
    }

    /// <summary>
    /// Get the config instance for the namespace. </summary>
    /// <param name="namespaces"> the namespaces of the config, order desc. </param>
    /// <returns> config instance </returns>
    public Task<IConfig> GetConfig(params string[] namespaces) => GetConfig((IEnumerable<string>)namespaces);

    /// <summary>
    /// Get the config instance for the namespace. </summary>
    /// <param name="namespaces"> the namespaces of the config, order desc. </param>
    /// <returns> config instance </returns>
    public async Task<IConfig> GetConfig(IEnumerable<string> namespaces)
    {
        if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));

        return new MultiConfig(await Task.WhenAll(namespaces.Reverse().Distinct().Select(GetConfig)).ConfigureAwait(false));
    }
}

internal class ApolloConfigurationManagerHelper
{
    private static IConfigManager? _manager;

    public static IConfigManager Manager => _manager ?? throw new InvalidOperationException("Please invoke 'AddApollo(options)' init apollo at the beginning.");

    internal static void SetApolloOptions(ConfigRepositoryFactory factory) =>
        Interlocked.CompareExchange(ref _manager, new DefaultConfigManager(new DefaultConfigRegistry(), factory), null);
}
