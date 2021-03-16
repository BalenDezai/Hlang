using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represents and executes a literal expression
    /// </summary>
    public class Literal : Expr
    {
        public object Value { get; set; }
        public Literal(object value)
        {
            Value = value;
        }
        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}
