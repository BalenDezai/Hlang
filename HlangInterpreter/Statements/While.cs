using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Contain and execute a while statement
    /// </summary>
    public class While : Statement
    {
        /// <summary>
        /// Condition of the while to be met
        /// </summary>
        public Expr Condition{ get; set; }
        /// <summary>
        /// Block statement to execute if condition is met
        /// </summary>
        public Statement Body { get; set; }
        public While(Expr condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }
}
