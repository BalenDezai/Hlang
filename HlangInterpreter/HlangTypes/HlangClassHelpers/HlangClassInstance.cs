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
                var foundField = Fields[name.Lexeme];
                if (foundField.IsPrivate) throw new RuntimeError(name, $"'{name.Lexeme}' is inaccessible due to it's protection level");
                return foundField.Value;
            }
            HlangFunction method = GetMethod(name);
            if (method != null) return method.Bind(this);

            if (ParentClass != null) return ParentClass.Get(name);

            throw new RuntimeError(name, $"Can't get undefined property '{name.Lexeme}'");
        }


        public override void Set(Token name, object value)
        {
            if (Fields.ContainsKey(name.Lexeme))
            {
                if (Fields[name.Lexeme].IsPrivate) throw new RuntimeError(name, $"'{name.Lexeme}' is inaccessible due to it's protection level");
                Fields[name.Lexeme].Value = value;
                return;
            }
            else if (ParentClass != null && ParentClass.SetParent(name, value))
            {
                return;
            }
            else
            {
                Fields.Add(name.Lexeme, new ClassField(name.Lexeme, value));
            }

        }

        public bool SetParent(Token name, object value)
        {
            if (Fields.ContainsKey(name.Lexeme))
            {
                if (Fields[name.Lexeme].IsPrivate) throw new RuntimeError(name, $"'{name.Lexeme}' is inaccessible due to it's protection level");
                Fields[name.Lexeme].Value = value;
                return true;
            }
            else if (ParentClass != null)
            {
                return ParentClass.SetParent(name, value);
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{InstanceOf} instance";
        }
    }
}
