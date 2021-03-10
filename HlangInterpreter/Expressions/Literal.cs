using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Expressions
{
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
