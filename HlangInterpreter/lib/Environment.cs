using HlangInterpreter.Errors;
using System.Collections.Generic;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Code environment. Keep track values in a scope and parent scopes
    /// </summary>
    public class Environment
    {
        // parent scope
        public Environment Parent { get; set; } = null;
        // contains the variables in this scope
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

        public Environment()
        {

        }
        public Environment(Environment parent)
        {
            Parent = parent;
        }
        /// <summary>
        /// Add a variable to current scope
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        public void Add(string name, object value)
        {
            Values.Add(name, value);
        }
        /// <summary>
        /// Check if a variable exists in this scope or outter scopes
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool VariableExists(string name)
        {
            if (Values.ContainsKey(name))
            {
                return true;
            }
            // if there is an outter scope, check that too
            if (Parent != null) return Parent.VariableExists(name);

            return false;
        }
        /// <summary>
        /// Get the value of a variable in this or outter scope
        /// </summary>
        /// <param name="name">Name of variable to get value of</param>
        /// <returns>Value of found variable</returns>
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
        /// <summary>
        /// Re-assign a variable in this or outter scopes
        /// </summary>
        /// <param name="name">Name of variable to re-assign</param>
        /// <param name="value">New value to assign</param>
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
