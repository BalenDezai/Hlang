using HlangInterpreter.Errors;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes.HlangClassHelpers
{
    public class HlangClassDeclaration : HlangClass, ICallable
    {
        private int _argumentLength;
        public string Name { get; set; }
        public HlangClass ParentClass { get; set; }
       
        public int ArgumentLength
        {
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

        public HlangClassDeclaration(string name)
        {
            Name = name;
        }
        public HlangClassDeclaration(string name, HlangClass parentClass)
        {
            Name = name;
            ParentClass = parentClass;
        }

        public override string ToString()
        {
            return Name;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new HlangClassInstance(Name)
            {
                Fields = Fields,
                Methods = Methods
            };
            HlangFunction init = GetMethod(Name);
            if (init != null)
            {
                init.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public override object Get(Token name)
        {
            if (ClassEnv.Values.ContainsKey(name.Lexeme))
            {
                return ClassEnv.Values[name.Lexeme];
            }
            HlangFunction method = GetMethod(name.Lexeme);
            if (method != null) return method;

            if (ParentClass != null) return ParentClass.Get(name);

            throw new RuntimeError(name, $"Can't get undefined static property '{name.Lexeme}'");
        }

        public override void Set(Token name, object value)
        {
            if (ClassEnv.Values.ContainsKey(name.Lexeme))
            {
                ClassEnv.Values[name.Lexeme] = value;
                return;
            }
            else if (ParentClass != null)
            {
                ParentClass.Set(name, value);
                return;
            }
            else
            {
                throw new RuntimeError(name, $"Can't set undefined static property '{name.Lexeme}'");
            }
        }

        public override HlangFunction GetMethod(string name)
        {
            if (Methods.ContainsKey(name))
            {
                return Methods[name];
            }
            return null;
        }
    }
}
