using HlangInterpreter.HelperInterfaces;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    public class Block : Statement
    {
        public List<Statement> Statements { get; set; }
        public Block(List<Statement> statements)
        {
            Statements = statements;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBlockStatement(this);
        }
    }
}
