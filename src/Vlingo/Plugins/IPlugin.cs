namespace Vlingo.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        void Close();
        void Start(IRegistrar registrar, string name, PluginProperties properties);
    }
}