using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    public class Logical : Expr
    {
        public Logical(Expr left, Token opr, Expr right)
        {
            Left = left;
            Operator = opr;
            Right = right;
        }
        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }
    }
}
