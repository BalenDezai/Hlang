using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    public class This : Expr
    {
        public Token Keyword { get; set; }
        public This(Token keyword)
        {
            Keyword = keyword;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitThisExpr(this);
        }
    }
}
