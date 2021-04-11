using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes.HlangClassHelpers
{
    public abstract class HlangClass
    {
        public Dictionary<string, HlangFunction> Methods { get; set; } = new Dictionary<string, HlangFunction>();
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
        public Environment ClassEnv { get; set; } = new Environment();
        public abstract HlangFunction GetMethod(string name);
        public abstract object Get(Token name);
        public abstract void Set(Token name, object value);
    }
}
