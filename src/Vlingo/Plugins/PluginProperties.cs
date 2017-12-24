using System;
using System.Text;

namespace Vlingo.Plugins
{
    public class PluginProperties
    {
        private string _name;
        private Properties _properties;

        public PluginProperties(String name, Properties properties)
        {
            _name = name;
            _properties = properties;
        }

        

        //public bool GetBoolean(string key, bool defaultValue)
        //{
        //    string value = getString(key, defaultValue.toString());
        //    return Boolean.parseBoolean(value);
        //}

        //public Float getFloat(String key, Float defaultValue)
        //{
        //    String value = getString(key, defaultValue.toString());
        //    return Float.parseFloat(value);
        //}

        //public Integer getInteger(String key, Integer defaultValue)
        //{
        //    String value = getString(key, defaultValue.toString());
        //    return Integer.parseInt(value);
        //}

        //public String getString(String key, String defaultValue)
        //{
        //    return properties.getProperty(key(key), defaultValue);
        //}

        //private String key(String key)
        //{
        //    return "plugin." + name + "." + key;
        //}
    }
}
