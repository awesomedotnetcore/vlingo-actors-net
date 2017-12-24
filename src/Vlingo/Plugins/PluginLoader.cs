using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vlingo.Plugins;

namespace Vlingo.Plugins
{
    public class PluginLoader
    {

        private const string PropertiesFile = "/vlingo-actors.properties";
        private const string PluginNamePrefix = "plugin.name.";

        public static void LoadPlugins(IRegistrar registrar)
        {
            new PluginLoader().LoadEnabledPlugins(registrar);
        }

        private PluginLoader() { }

        private ISet<String> FindEnabledPlugins(Properties properties)
        {
            ISet<String> enabledPlugins = new HashSet<String>();

            //for (Enumeration <?> e = properties.keys(); e.hasMoreElements();)
            //{
            //    final String key = (String)e.nextElement();
            //    if (key.startsWith(pluginNamePrefix))
            //    {
            //        if (Boolean.parseBoolean(properties.getProperty(key)))
            //            enabledPlugins.add(key);
            //    }
            //}

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

        private Properties LoadProperties()
        {
            Properties properties = new Properties();

            try
            {
                // properties.load(PluginLoader.class.getResourceAsStream(propertiesFile));
            }
            catch (IOException e)
            {
                throw new InvalidOperationException($"Must provide properties file on classpath: {PropertiesFile}");
            }

            return properties;
        }

        private void RegisterPlugin(IRegistrar registrar, Properties properties, string enabledPlugin)
        {
            String pluginName = enabledPlugin.Substring(PluginNamePrefix.Length);
            String classnameKey = $"plugin.{pluginName}.classname";
            String classname = "";// properties.getProperty(classnameKey);

            try
            {
                Type pluginClass = Type.GetType(classname);
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

    public interface IPlugin
    {
        void Close();
        string Name { get; }
        void Start(IRegistrar registrar, string name, PluginProperties properties);
    }
}
