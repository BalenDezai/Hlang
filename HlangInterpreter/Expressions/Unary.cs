using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represents and executes an unary expression
    /// </summary>
    public class Unary : Expr
    {
        public Unary(Token opr, Expr right = null, Expr left = null)
        {
            Operator = opr;
            Right = right;
            Left = left;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
