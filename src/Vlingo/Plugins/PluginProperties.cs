using System.Globalization;
using Vlingo.Infra;

namespace Vlingo.Plugins
{
    public class PluginProperties
    {
        private readonly string _name;
        private readonly Properties _properties;

        public PluginProperties(string name, Properties properties)
        {
            _name = name;
            _properties = properties;
        }


        public bool GetBoolean(string key, bool defaultValue)
        {
            string value = Getstring(key, defaultValue.ToString());
            return bool.Parse(value);
        }

        public float GetFloat(string key, float defaultValue)
        {
            string value = Getstring(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.Parse(value);
        }

        public int GetInteger(string key, int defaultValue)
        {
            string value = Getstring(key, defaultValue.ToString());
            return int.Parse(value);
        }

        public string Getstring(string key, string defaultValue)
        {
            return _properties.GetProperty(Key(key), defaultValue);
        }

        private string Key(string key)
        {
            return $"plugin.{_name}.{key}";
        }
    }
}