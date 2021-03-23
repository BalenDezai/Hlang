using HlangInterpreter.Enums;
using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Statements;
using System;
using System.Collections.Generic;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Semantic analysis.
    /// Also catch errors  before interpreter is executed
    /// </summary>
    public class SemanticAnalyzer : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private readonly Stack<Dictionary<string, bool>> _scopes;
        private readonly Interpreter _interpreter;
        private FunctionType _currentFuncType;
        private readonly Stack<CallingBody> _callingBodyStack;

        public SemanticAnalyzer(Interpreter interpreter)
        {
            _interpreter = interpreter;
            _callingBodyStack = new Stack<CallingBody>();
            _callingBodyStack.Push(CallingBody.NONE);
            _scopes = new Stack<Dictionary<string, bool>>();
            _scopes.Push(new Dictionary<string, bool>());
            _currentFuncType = FunctionType.NONE;
        }

        public void Analyze(List<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Statement statement)
        {
            statement.Accept(this);
        }

        private object Resolve(Expr expression)
        {
            return expression.Accept(this);
        }

        public object VisitAssignExpr(Assign expr)
        {
            Resolve(expr.Value);
            return null;
        }

        public object VisitBinaryExpr(Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object VisitBlockStatement(Block statement)
        {
            BeginSCope();
            Analyze(statement.Statements);
            EndScope();
            return null;
        }

        public object VisitBreakStatement(Break statement)
        {
            if (_callingBodyStack.Peek() == CallingBody.FOR_EACH || _callingBodyStack.Peek() == CallingBody.WHILE)
            {
                return null;
            }
            throw new Exception("NOT IN WHILE OR FOR EACH");
        }

        public object VisitExpressionStatement(Expression statement)
        {
            Resolve(statement.Expr);
            return null;
        }

        public object VisitForeachStatement(ForEach statement)
        {
            _callingBodyStack.Push(CallingBody.FOR_EACH);
            Resolve(statement.Identifier);
            Resolve(statement.List);
            Resolve(statement.Block);
            _callingBodyStack.Pop();
            return null;
        }

        public object VisitFunctionCallExpr(FunctionCall expr)
        {
            Resolve(expr.Callee);
            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }
            return null;
        }

        public object visitFunctionStatement(Function statement)
        {
            Define(statement.Name);
            ResolveFunction(statement, FunctionType.FUNCITON, CallingBody.FUNCTION);
            return null;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object VisitIfStatement(If statement)
        {
            Resolve(statement.Condition);
            Resolve(statement.ThenBranch);
            if (statement.ElseBranch != null) Resolve(statement.ElseBranch);
            return null;
        }

        public object VisitLambdaExpr(Lambda expr)
        {
            Resolve(expr.Body);
            return null;

        }

        public object VisitListExpr(List expr)
        {
            foreach (var item in expr.Values)
            {
                Resolve(item);
            }
            return null;
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object VisitPrintStatement(Print statement)
        {
            Resolve(statement.Expression);
            return null;
        }

        public object VisitReturnStatement(Return statement)
        {
            if (statement.Value != null)
            {
                Resolve(statement.Value);
            }
            return null;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            if (expr.Operator.Type == TokenType.COMPLEMENT)
            {
                double right = (double)Resolve(expr.Right);
                bool isNonDecimal = Math.Abs(right % 1) <= (double.Epsilon * 100);
                if (!isNonDecimal)
                {
                    throw new Exception();
                }
            }
            return null;
        }

        public object VisitVariableExpr(Variable expr)
        {
            if (!_scopes.Peek().ContainsKey(expr.Name.Lexeme))
            {
                Define(expr.Name);
            }
            foreach (var item in _scopes)
            {
                if (item.ContainsKey(expr.Name.Lexeme))
                {
                    return null;
                }
            }
            throw new Exception();
        }

        public object VisitWhileStatement(While statement)
        {
            _callingBodyStack.Push(CallingBody.WHILE);
            Resolve(statement.Condition);
            Resolve(statement.Body);
            _callingBodyStack.Pop();
            return null;
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0) return;
            if (!_scopes.Peek().ContainsKey(name.Lexeme))
            {
                _scopes.Peek().Add(name.Lexeme, false);
            }
        }

        private void BeginSCope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            _scopes.Pop();
        }

        private void ResolveFunction(Function statement, FunctionType type, CallingBody callingBody)
        {
            FunctionType enclosing = _currentFuncType;
            _callingBodyStack.Push(callingBody);
            _currentFuncType = type;
            BeginSCope();
            foreach (var parameter in statement.Paramters)
            {
                Define(parameter);
            }
            Analyze(statement.Body);
            EndScope();
            _currentFuncType = enclosing;
            _callingBodyStack.Pop();
        }
    }
}
