using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    public class Function : Statement
    {
        public Token Name { get; set; }
        public List<Token> Paramters { get; set; }
        public List<Statement> Body { get; set; }

        public Function(Token name, List<Token> paramters, List<Statement> body)
        {
            Name = name;
            Paramters = paramters;
            Body = body;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.visitFunctionStatement(this);
        }
    }
}
