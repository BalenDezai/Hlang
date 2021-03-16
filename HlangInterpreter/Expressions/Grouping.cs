using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represent and execute a grouping expression
    /// </summary>
    public class Grouping : Expr
    {
        public Expr Expression { get; set; }
        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
}
