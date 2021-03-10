using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    public class Return : Statement
    {
        public Expr Value { get; set; }

        public Return(Expr value)
        {
            Value = value;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }
    }
}
