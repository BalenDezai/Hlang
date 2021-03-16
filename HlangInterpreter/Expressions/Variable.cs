using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represents and executes a variable expression
    /// </summary>
    public class Variable : Expr
    {
        public Token Name { get; set; }
        public Variable(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}
