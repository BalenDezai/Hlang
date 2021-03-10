using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    public class While : Statement
    {
        public Expr Condition{ get; set; }
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
