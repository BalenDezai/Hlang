using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    public class ForEach : Statement
    {
        public Variable Identifier { get; set; }
        public Expr List { get; set; }
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
