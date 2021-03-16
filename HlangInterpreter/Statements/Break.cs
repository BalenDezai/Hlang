using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contain and execute a break statement
    /// </summary>
    public class Break : Statement
    {
        public Token Keyword { get; set; }
        public Break(Token keyword)
        {
            Keyword = keyword;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBreakStatement(this);
        }
    }
}
