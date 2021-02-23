using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;

namespace HlangInterpreter.ExprLib
{
    public class Assign : Expr
    {
        public Token Name { get; set; }
        public Expr Value { get; set; }

        public Assign(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }
}
