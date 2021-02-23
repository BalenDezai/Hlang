using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;

namespace HlangInterpreter.ExprLib
{
    public abstract class Expr
    {
        public Expr Left { get; set; }
        public Token Operator { get; set; }
        public Expr Right { get; set; }

        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }
}
