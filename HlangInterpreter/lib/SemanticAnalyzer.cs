using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace HlangInterpreter.Lib
{
    public class SemanticAnalyzer : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private readonly Interpreter _interpreter;

        public SemanticAnalyzer(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }
        public object VisitAssignExpr(Assign expr)
        {
            throw new NotImplementedException();
        }

        public object VisitBinaryExpr(Binary expr)
        {
            throw new NotImplementedException();
        }

        public object VisitBlockStatement(Block statement)
        {
            throw new NotImplementedException();
        }

        public object VisitBreakStatement(Break statement)
        {
            throw new NotImplementedException();
        }

        public object VisitExpressionStatement(Expression statement)
        {
            throw new NotImplementedException();
        }

        public object VisitForeachStatement(ForEach statement)
        {
            throw new NotImplementedException();
        }

        public object VisitFunctionCallExpr(FunctionCall expr)
        {
            throw new NotImplementedException();
        }

        public object visitFunctionStatement(Function statement)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            throw new NotImplementedException();
        }

        public object VisitIfStatement(If statement)
        {
            throw new NotImplementedException();
        }

        public object VisitLambdaExpr(Lambda expr)
        {
            throw new NotImplementedException();
        }

        public object VisitListExpr(List expr)
        {
            throw new NotImplementedException();
        }

        public object VisitLiteralExpr(Literal expr)
        {
            throw new NotImplementedException();
        }

        public object VisitLogicalExpr(Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitPrintStatement(Print statement)
        {
            throw new NotImplementedException();
        }

        public object VisitReturnStatement(Return statement)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpr(Unary expr)
        {
            throw new NotImplementedException();
        }

        public object VisitVariableExpr(Variable expr)
        {
            throw new NotImplementedException();
        }

        public object VisitWhileStatement(While statement)
        {
            throw new NotImplementedException();
        }
    }
}
