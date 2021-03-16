using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contain and execute an expression
    /// </summary>
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
