using HlangInterpreter.ExprLib;

namespace HlangInterpreter.HelperInterfaces
{
    public interface IExprVisitor<T>
    {
        T VisitAssignExpr(Assign expr);
        T VisitVariableExpr(Variable expr);
        T VisitLiteralExpr(Literal expr);
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitUnaryExpr(Unary expr);
    }
}
