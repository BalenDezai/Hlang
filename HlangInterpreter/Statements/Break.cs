using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.lib;
using System;

namespace HlangInterpreter.Statements
{
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
