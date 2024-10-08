﻿using Com.Ctrip.Framework.Apollo.Model;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace Com.Ctrip.Framework.Apollo;

public abstract class ApolloConfigurationBuilder : ConfigurationBuilder
{
    private static readonly object Lock = new();
    private static readonly FieldInfo ConfigurationManagerReset = typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static)!;
    public static bool AppSettingsInitialized { get; private set; }

    private IConfig? _config;
    public IReadOnlyList<string>? Namespaces { get; private set; }
    public string? SectionName { get; private set; }

    public override void Initialize(string name, NameValueCollection config)
    {
        Namespaces = config["namespace"]?.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (this is not AppSettingsSectionBuilder)
        {
            _ = ConfigurationManager.AppSettings; //App Settings must be initialized first

            AppSettingsInitialized = true;
        }

        base.Initialize(name, config);
    }

    public override XmlNode ProcessRawXml(XmlNode rawXml)
    {
        SectionName = rawXml.Name;

        return base.ProcessRawXml(rawXml);
    }

    protected IConfig GetConfig()
    {
        if (_config != null) return _config;

        lock (Lock)
        {
            Interlocked.MemoryBarrier();

            if (_config != null) return _config;

            Task<IConfig> config;
            if (Namespaces == null || Namespaces.Count == 0)
#pragma warning disable 618
                config = ApolloConfigurationManager.GetAppConfig();
            else if (Namespaces.Count == 1)
                config = ApolloConfigurationManager.GetConfig(Namespaces[0]);
            else
                config = ApolloConfigurationManager.GetConfig(Namespaces);
#pragma warning restore 618
            _config = config.ConfigureAwait(false).GetAwaiter().GetResult();

            _config.ConfigChanged += Config_ConfigChanged;
        }

        return _config;
    }

    private void Config_ConfigChanged(IConfig config, ConfigChangeEventArgs args)
    {
        try
        {
            ConfigurationManagerReset.SetValue(null, 0);

            config.ConfigChanged -= Config_ConfigChanged;
        }
        catch
        {
            ConfigurationManager.RefreshSection(SectionName!);
        }
    }
}