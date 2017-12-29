using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vlingo.Infra;

namespace Vlingo.Plugins
{
    public class PluginLoader
    {
        private const string PropertiesFile = "vlingo-actors.properties";
        private const string PluginNamePrefix = "plugin.name.";

        private PluginLoader()
        {
        }

        public static void LoadPlugins(IRegistrar registrar)
        {
            new PluginLoader().LoadEnabledPlugins(registrar);
        }

        private static IEnumerable<string> FindEnabledPlugins(Properties properties)
        {
            ISet<string> enabledPlugins = new HashSet<string>();

            foreach (var p in properties)
            {
                var key = p.Key;
                if (!key.StartsWith(PluginNamePrefix))
                {
                    continue;
                }

                var value = p.Value;
                if (bool.TryParse(value, out var enabled) && enabled)
                {
                    enabledPlugins.Add(key);
                }
            }

            return enabledPlugins;
        }

        private void LoadEnabledPlugins(IRegistrar registrar)
        {
            var properties = LoadProperties();
            foreach (var enabledPlugin in FindEnabledPlugins(properties))
            {
                RegisterPlugin(registrar, properties, enabledPlugin);
            }
        }

        private static Properties LoadProperties()
        {
            var properties = new Properties();

            try
            {
                var assembly = Assembly.GetEntryAssembly();
                using (var stream = assembly.GetManifestResourceStream(PropertiesFile))
                {
                    properties.Load(stream);
                }
            }
            catch (IOException e)
            {
                throw new InvalidOperationException($"Must provide properties file on classpath: {PropertiesFile}");
            }

            return properties;
        }

        private void RegisterPlugin(IRegistrar registrar, Properties properties, string enabledPlugin)
        {
            var pluginName = enabledPlugin.Substring(PluginNamePrefix.Length);
            var classnameKey = $"plugin.{pluginName}.classname";
            var classname = properties.GetProperty(classnameKey);

            try
            {
                var pluginClass = Type.GetType(classname);
                var plugin = Activator.CreateInstance(pluginClass) as IPlugin;
                if (plugin == null)
                {
                    throw new ArgumentNullException(nameof(plugin));
                }
                plugin.Start(registrar, pluginName, new PluginProperties(pluginName, properties));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot load plugin {classname}");
            }
        }
    }
}