using HlangInterpreter.Expressions;

namespace HlangInterpreter.HelperInterfaces
{
    public interface IExpressionVisitor<T>
    {
        T VisitAssignExpr(Assign expr);
        T VisitVariableExpr(Variable expr);
        T VisitLiteralExpr(Literal expr);
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitUnaryExpr(Unary expr);
        T VisitLogicalExpr(Logical expr);
        T VisitListExpr(List expr);
        T VisitFunctionCallExpr(FunctionCall expr);
        T VisitLambdaExpr(Lambda expr);
    }
}
