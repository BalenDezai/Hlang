using HlangInterpreter.StmtLib;

namespace HlangInterpreter.HelperInterfaces
{
    public interface IStatementVisitor<T>
    {
        T VisitPrintStatement(Print statement);
        T VisitExpressionStatement(Expression statement);
        T VisitBlockStatement(Block statement);
    }
}
