using HlangInterpreter.Errors;
using System.Collections.Generic;

namespace HlangInterpreter.lib
{
    public class Environment
    {
        public Environment Parent { get; set; } = null;
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

        public Environment()
        {

        }
        public Environment(Environment parent)
        {
            Parent = parent;
        }

        public void Add(string name, object value)
        {
            Values.Add(name, value);
        }

        public bool VariableExists(string name)
        {
            if (Values.ContainsKey(name))
            {
                return true;
            }

            if (Parent != null) return Parent.VariableExists(name);

            return false;
        }

        public object GetValue(Token name)
        {
            if (Values.ContainsKey(name.Lexeme))
            {
                return Values[name.Lexeme];
            }
            // recursively look through parent environments to find the variable
            if (Parent != null) return Parent.GetValue(name);

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }

        public void Assign(Token name, object value)
        {
            if (Values.ContainsKey(name.Lexeme))
            {
                Values[name.Lexeme] = value;
                return;
            }
            // recursively look through parent environment to assign to proper variable
            if (Parent != null)
            {
                Parent.Assign(name, value);
                return;
            }
            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }
    }
}
