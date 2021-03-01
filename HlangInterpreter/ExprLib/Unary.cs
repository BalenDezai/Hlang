using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;

namespace HlangInterpreter.ExprLib
{
    public class Unary : Expr
    {
        public Unary(Token opr, Expr right)
        {
            Operator = opr;
            Right = right;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
