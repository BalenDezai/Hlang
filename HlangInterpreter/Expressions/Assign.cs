using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represent and execute an assignment expression
    /// </summary>
    public class Assign : Expr
    {
        public Token Name { get; set; }
        public Expr Value { get; set; }

        public Assign(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }
}
