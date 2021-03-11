﻿using HlangInterpreter.Statements;

namespace HlangInterpreter.HelperInterfaces
{
    public interface IStatementVisitor<T>
    {
        T VisitPrintStatement(Print statement);
        T VisitExpressionStatement(Expression statement);
        T VisitBlockStatement(Block statement);
        T VisitIfStatement(If statement);
        T VisitWhileStatement(While statement);
        T VisitForeachStatement(ForEach statement);
        T visitFunctionStatement(Function statement);
        T VisitReturnStatement(Return statement);
        T VisitBreakStatement(Break statement);
    }
}