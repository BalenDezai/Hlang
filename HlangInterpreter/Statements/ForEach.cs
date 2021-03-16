using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contain and execute a for each statement
    /// </summary>
    public class ForEach : Statement
    {
        /// <summary>
        /// Argument identifier
        /// </summary>
        public Variable Identifier { get; set; }
        /// <summary>
        /// List to run the for each on
        /// </summary>
        public Expr List { get; set; }
        /// <summary>
        /// Block of statements to execute
        /// </summary>
        public Block Block { get; set; }
        public ForEach(Variable identifer, Expr list, Block block)
        {
            Identifier = identifer;
            Block = block;
            List = list;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitForeachStatement(this);
        }
    }
}
