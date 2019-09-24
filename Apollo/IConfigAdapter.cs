﻿using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo
{
    public interface IConfigAdapter
    {
        Properties GetProperties(Properties properties);
    }

    public abstract class ContentConfigAdapter : IConfigAdapter
    {
        private static readonly Func<Action<LogLevel, string, Exception>> Logger = () => LogManager.CreateLogger(typeof(ContentConfigAdapter));

        public Properties GetProperties(Properties properties)
        {
            var content = properties.GetProperty(ConfigConsts.ConfigFileContentKey);
            if (string.IsNullOrWhiteSpace(content))
            {
                Logger().Warn($"Can not find '{ConfigConsts.ConfigFileContentKey}'.");

                return properties;
            }

            return GetProperties(content);
        }

        public abstract Properties GetProperties([NotNull]string content);
    }

    public static class ConfigAdapterRegister
    {
        private static readonly IDictionary<ConfigFileFormat, IConfigAdapter> Dic;

        static ConfigAdapterRegister()
        {
            Dic = new Dictionary<ConfigFileFormat, IConfigAdapter>();

            AddAdapter(ConfigFileFormat.Json, new JsonConfigAdapter());
            AddAdapter(ConfigFileFormat.Xml, new XmlConfigAdapter());
        }

        public static void AddAdapter(ConfigFileFormat format, [NotNull]IConfigAdapter adapter) =>
            Dic[format] = adapter ?? throw new ArgumentNullException(nameof(adapter));

        internal static bool TryGetAdapter(ConfigFileFormat format, out IConfigAdapter adapter) =>
            Dic.TryGetValue(format, out adapter);
    }
}

