using HlangInterpreter.Errors;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes
{
    public class HlangClass : ICallable
    {
        private int _argumentLength;
        public string Name { get; set; }
        public HlangClass ParentClass { get; set; }
        public Dictionary<string, HlangFunction> Methods { get; set; }
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
        public int ArgumentLength {
            get
            {
                HlangFunction init = GetMethod(Name);
                if (init == null) return _argumentLength;
                return init.ArgumentLength;
            }
            set 
            {
                _argumentLength = value;
            }
        }

        public HlangClass(string name)
        {
            Name = name;
        }
        public HlangClass(string name, HlangClass parentClass)
        {
            Name = name;
            ParentClass = parentClass;
        }

        public object Get(Token name)
        {
            if (Fields.ContainsKey(name.Lexeme))
            {
                return Fields[name.Lexeme];
            }
            HlangFunction method = GetMethod(name.Lexeme);
            if (method != null) return method;

            if (ParentClass != null) return ParentClass.Get(name);

            throw new RuntimeError(name, $"Undefined property {name.Lexeme}");
        }

        public void Set(Token name, object value)
        {
            if (Fields.ContainsKey(name.Lexeme))
            {
                Fields[name.Lexeme] = value;
                return;
            }
            else if (ParentClass != null)
            {
                ParentClass.Set(name, value);
                return;
            }
            else
            {
                Fields.Add(name.Lexeme, value);
                return;
            }

        }

        public override string ToString()
        {
            return Name;
        }

        public HlangFunction GetMethod(string name)
        {
            if (Methods.ContainsKey(name))
            {
                if (Methods[name] is HlangFunction) return (HlangFunction)Methods[name];
            }
            else if (ParentClass != null)
            {
                return ParentClass.GetMethod(name);
            }
            return null;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            
            HlangFunction init = GetMethod(Name);
            if (init != null)
            {
                init.Call(interpreter, arguments);
            }
            return this;
        }
    }
}
