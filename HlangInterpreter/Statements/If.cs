using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    public class If : Statement
    {
        public Expr Condition { get; set; }
        public Statement ThenBranch { get; set; }
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
