using HlangInterpreter.Errors;
using HlangInterpreter.Lib;

namespace HlangInterpreter.HlangTypes.HlangClassHelpers
{
    public class HlangClassInstance : HlangClass
    {
        public string InstanceOf { get; private set; }
        public HlangClassInstance ParentClass { get; set; }

        public HlangClassInstance(string instanceOf)
        {
            InstanceOf = instanceOf;
        }

        public override object Get(Token name)
        {
            if (Fields.ContainsKey(name.Lexeme))
            {
                return Fields[name.Lexeme];
            }
            HlangFunction method = GetMethod(name.Lexeme);
            if (method != null) return method.Bind(this);

            if (ParentClass != null) return ParentClass.Get(name);

            throw new RuntimeError(name, $"Can't get undefined property '{name.Lexeme}'");
        }

        public override void Set(Token name, object value)
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

        public override HlangFunction GetMethod(string name)
        {
            if (Methods.ContainsKey(name))
            {
                if (Methods[name] is HlangFunction) return Methods[name];
            }
            else if (ParentClass != null)
            {
                return ParentClass.GetMethod(name);
            }
            return null;
        }

        public override string ToString()
        {
            return $"{InstanceOf} instance";
        }
    }
}
