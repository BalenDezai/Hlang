using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    public class GetProperty : Expr
    {
        public Token Name { get; set; }
        public Expr Object { get; set; }
        public GetProperty(Expr obj, Token name)
        {
            Object = obj;
            Name = name;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitGetPropertyExpr(this);
        }
    }
}
