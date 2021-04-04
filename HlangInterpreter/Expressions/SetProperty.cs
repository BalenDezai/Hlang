using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    public class SetProperty : Expr
    {
        public Expr Object { get; set; }
        public Token Name { get; set; }
        public Expr Value { get; set; }
        public SetProperty(Expr obj, Token name, Expr value)
        {
            Object = obj;
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitSetPropertyExpr(this);
        }
    }
}
