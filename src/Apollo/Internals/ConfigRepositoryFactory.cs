﻿using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util.Http;

namespace Com.Ctrip.Framework.Apollo.Internals;

public class ConfigRepositoryFactory : IConfigRepositoryFactory, IDisposable
{
    private readonly HttpUtil _httpUtil;
    private readonly ConcurrentDictionary<string, IConfigRepository> _configRepositories = new();
    private readonly IApolloOptions _options;
    private readonly RemoteConfigLongPollService _remoteConfigLongPollService;
    private readonly ConfigServiceLocator _serviceLocator;

    public ConfigRepositoryFactory(IApolloOptions options, HttpUtil? httpUtil = null)
    {
        _options = options;
        _httpUtil = httpUtil ?? new HttpUtil(options);
        _serviceLocator = new(_httpUtil, _options);
        _remoteConfigLongPollService = new(_serviceLocator, _httpUtil, _options);
    }

    public IConfigRepository GetConfigRepository(string @namespace) =>
        _configRepositories.GetOrAdd(@namespace, CreateConfigRepository);

    private IConfigRepository CreateConfigRepository(string @namespace)
    {
        if (Env.Local.Equals(_options.Env))
        {
            LogManager.CreateLogger(typeof(ConfigRepositoryFactory)).Warn($"==== Apollo is in local mode! Won't pull configs '{@namespace}' from remote server! ====");
            return new LocalFileConfigRepository(@namespace, _options);
        }
        var remoteRepo = new RemoteConfigRepository(@namespace, _options, _httpUtil, _serviceLocator, _remoteConfigLongPollService);
        if (!_options.EnableLocalFileCache)
        {
            return remoteRepo;
        }
        return new LocalFileConfigRepository(@namespace, _options, remoteRepo);
    }

    public void Dispose()
    {
        _remoteConfigLongPollService.Dispose();
        _serviceLocator.Dispose();
        _httpUtil.Dispose();
    }
}
