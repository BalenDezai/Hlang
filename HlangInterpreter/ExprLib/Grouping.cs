using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.ExprLib
{
    public class Grouping : Expr
    {
        public Expr Expression { get; set; }
        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
}
