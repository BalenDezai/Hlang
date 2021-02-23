using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;

namespace HlangInterpreter.ExprLib
{
    public class Variable : Expr
    {
        public Token Name { get; set; }
        public Variable(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}
