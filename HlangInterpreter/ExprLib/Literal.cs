using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.ExprLib
{
    public class Literal : Expr
    {
        public object Value { get; set; }
        public Literal(object value)
        {
            Value = value;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}
