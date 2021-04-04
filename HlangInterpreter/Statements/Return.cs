using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Executes the return statement
    /// </summary>
    public class Return : Statement
    {
        public Expr Value { get; set; }
        public Token Keyword { get; set; }
        public Return(Token keyword, Expr value)
        {
            Keyword = keyword;
            Value = value;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }
    }
}
