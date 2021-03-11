using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    public class Unary : Expr
    {
        public Unary(Token opr, Expr right)
        {
            Operator = opr;
            Right = right;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
