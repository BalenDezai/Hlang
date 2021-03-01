using HlangInterpreter.ExprLib;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.StmtLib
{
    public class Print : Statement
    {
        public Expr Expression { get; set; }
        public Print(Expr expression)
        {
            Expression = expression;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitPrintStatement(this);
        }
    }
}
