using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;

namespace HlangInterpreter.ExprLib
{
    public class Binary : Expr
    {
        public Binary(Expr left, Token opr, Expr right)
        {
            Left = left;
            Operator = opr;
            Right = right;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
}
