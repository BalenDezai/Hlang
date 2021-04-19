using HlangInterpreter.Errors;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes.HlangClassHelpers
{
    public abstract class HlangClass
    {
        public Dictionary<string, HlangFunction> Methods { get; set; } = new Dictionary<string, HlangFunction>();
        public Dictionary<string, ClassField> Fields { get; set; } = new Dictionary<string, ClassField>();
        public Environment ClassEnv { get; set; } = new Environment();
        public virtual HlangFunction GetMethod(Token name)
        {
            if (Methods.ContainsKey(name.Lexeme))
            {
                var foundFunc = Methods[name.Lexeme];
                if (foundFunc.IsPrivate) throw new RuntimeError(name, $"'{name.Lexeme}' is inaccessible due to it's protection level");
                return foundFunc;
            }
            return null;
        }
        public abstract object Get(Token name);
        public abstract void Set(Token name, object value);
    }
}
