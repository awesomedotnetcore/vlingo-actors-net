using System.Collections.Generic;

namespace Vlingo.Testkit
{
    public class TestState
    {
        private readonly Dictionary<string, object> _state;

        public TestState()
        {
            _state = new Dictionary<string, object>();
        }

        public TestState PutValue(string name, object value)
        {
            _state[name] = value;
            return this;
        }


        public T ValueOf<T>(string name)
        {
            return (T) _state[name];
        }
    }
}