using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contain and define a function
    /// </summary>
    public class Function : Statement
    {
        /// <summary>
        /// Function identifer
        /// </summary>
        public Token Name { get; set; }
        /// <summary>
        /// Function paramters
        /// </summary>
        public List<Token> Paramters { get; set; }
        /// <summary>
        /// Function body to execute
        /// </summary>
        public List<Statement> Body { get; set; }
        public bool IsStatic { get; set; }
        public Function(Token name, List<Token> paramters, List<Statement> body, bool isStatic = false)
        {
            Name = name;
            Paramters = paramters;
            Body = body;
            IsStatic = isStatic;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.visitFunctionStatement(this);
        }
    }
}
