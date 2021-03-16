using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contain and execute an if statement
    /// </summary>
    public class If : Statement
    {
        /// <summary>
        /// Condition of the if statement
        /// </summary>
        public Expr Condition { get; set; }
        /// <summary>
        /// Block statement to execute if condition is met
        /// </summary>
        public Statement ThenBranch { get; set; }
        /// <summary>
        /// else block statement to execute if condition is not met
        /// </summary>
        public Statement ElseBranch { get; set; }
        public If(Expr condition, Statement thenBranch, Statement elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }
    }
}
