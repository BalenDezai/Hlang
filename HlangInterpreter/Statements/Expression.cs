using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    public class Expression : Statement
    {
        public Expr Expr { get; set; }
        public Expression(Expr expression)
        {
            Expr = expression;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }
    }
}
