using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    public class Binary : Expr
    {
        public Binary(Expr left, Token opr, Expr right)
        {
            Left = left;
            Operator = opr;
            Right = right;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
}
