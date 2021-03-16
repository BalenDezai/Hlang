using HlangInterpreter.HelperInterfaces;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contains and executes a block statement
    /// </summary>
    public class Block : Statement
    {
        /// <summary>
        /// The statements to execute
        /// </summary>
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
