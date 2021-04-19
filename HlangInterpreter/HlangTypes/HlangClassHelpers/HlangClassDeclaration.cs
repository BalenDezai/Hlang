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
        public Dictionary<string, ClassField> EnvTracker { get; set; } = new Dictionary<string, ClassField>();
        public HlangClassDeclaration ParentClass { get; set; }
       
        public int ArgumentLength
        {
            get
            {
                if (Methods.TryGetValue(Name, out HlangFunction init))
                {
                    return init.ArgumentLength;
                }
                else
                {
                    return _argumentLength;
                }
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
        public HlangClassDeclaration(string name, HlangClassDeclaration parentClass)
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
            if (Methods.TryGetValue(Name, out HlangFunction init))
            {
                init.Bind(instance).Call(interpreter, arguments);
            }
            return instance;
        }

        public override object Get(Token name)
        {
            if (EnvTracker.ContainsKey(name.Lexeme))
            {
                var value = EnvTracker[name.Lexeme];
                if (value.IsPrivate) throw new RuntimeError(name, $"'{name.Lexeme}' is inaccessible due to it's protection level");
                return ClassEnv.Values[name.Lexeme];
            }

            if (ParentClass != null) return ParentClass.Get(name);

            throw new RuntimeError(name, $"Can't get undefined static property '{name.Lexeme}'");
        }

        public override void Set(Token name, object value)
        {
            if (EnvTracker.ContainsKey(name.Lexeme))
            {
                var val = EnvTracker[name.Lexeme];
                if (val.IsPrivate) throw new RuntimeError(name, $"'{name.Lexeme}' is inaccessible due to it's protection level");
                ClassEnv.Values[name.Lexeme] = value;
                return;
            }
            else if (ParentClass != null)
            {
                Set(name, value);
                return;
            }
            else
            {
                throw new RuntimeError(name, $"Can't set proeprty on a class declaration'{name.Lexeme}'");
            }
        }

    }
}
