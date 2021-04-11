using HlangInterpreter.Enums;
using HlangInterpreter.Errors;
using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.HlangTypes;
using HlangInterpreter.HlangTypes.HlangClassHelpers;
using HlangInterpreter.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Semantic analysis.
    /// Also catch errors  before interpreter is executed
    /// </summary>
    public class SemanticAnalyzer : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private readonly Stack<Dictionary<string, object>> _scopes;
        private Environment _env;
        private FunctionType _currentFuncType;
        private readonly Stack<CallingBody> _callingBodyStack;

        public SemanticAnalyzer()
        {
            _callingBodyStack = new Stack<CallingBody>();
            _callingBodyStack.Push(CallingBody.NONE);
            _env = new Environment();
            _scopes = new Stack<Dictionary<string, object>>();
            _scopes.Push(new Dictionary<string, object>());
            _currentFuncType = FunctionType.NONE;
        }

        public void Analyze(List<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                Resolve(statement);
            }
        }

        private object Resolve(Statement statement)
        {
            return statement.Accept(this);
        }

        private object Resolve(Expr expression)
        {
            return expression.Accept(this);
        }

        public object VisitAssignExpr(Assign expr)
        {
            var value = Resolve(expr.Value);
            Define(expr.Name, value);
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
            throw new SemanticError(statement.Keyword, "NOT IN WHILE OR FOR EACH");
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
            var x = Resolve(expr.Callee);
            if (x != null && x is ICallable)
            {
                int expectedArguments = ((ICallable)x).ArgumentLength;
                if (expectedArguments != expr.Arguments.Count)
                {
                    throw new SemanticError(expr.Keyword, $"Expected {expectedArguments} arguments but got {expr.Arguments.Count}");
                }
            }
            
            Resolve(expr.Callee);
            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }
            return null;
        }

        public object visitFunctionStatement(Function statement)
        {
            Define(statement.Name, new HlangFunction(statement, new Environment()));
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
            if (_currentFuncType == FunctionType.INIT) throw new SemanticError(statement.Keyword, "Can't 'return' inside of an initializer");
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
            return GetValue(expr.Name);
        }

        public object VisitWhileStatement(While statement)
        {
            _callingBodyStack.Push(CallingBody.WHILE);
            Resolve(statement.Condition);
            Resolve(statement.Body);
            _callingBodyStack.Pop();
            return null;
        }

        private void Define(Token name, object value)
        {
            _env.Add(name.Lexeme, value);
        }

        private object GetValue(Token name)
        {
            return _env.GetValue(name);
        }

        private void BeginSCope()
        {
            _env = new Environment(_env);
        }

        private void EndScope()
        {
            _env = _env.Parent;
        }

        private void ResolveFunction(Function statement, FunctionType type, CallingBody callingBody)
        {
            if (type ==  FunctionType.INIT && _callingBodyStack.Peek() == CallingBody.SUBCLASS)
            {
                
                Expr first = ((Expression)statement.Body.First()).Expr;
                if (!(first is Parent)) throw new SemanticError(statement.Name, "First expression in an initializer must be a inherited initializer call");
            }
            FunctionType enclosing = _currentFuncType;
            _callingBodyStack.Push(callingBody);
            _currentFuncType = type;
            BeginSCope();
            foreach (var parameter in statement.Paramters)
            {
                Define(parameter, parameter.Literal);
            }
            Analyze(statement.Body);
            EndScope();
            _currentFuncType = enclosing;
            _callingBodyStack.Pop();
        }

        public object VisitClassSTatement(Class statement)
        {
            _callingBodyStack.Push(CallingBody.CLASS);
            HlangClassDeclaration parentClass = null;
            var newClass = new HlangClassDeclaration(statement.Name.Lexeme);
            BeginSCope();

            if (statement.ParentClass != null)
            {

                if (statement.Name.Lexeme == statement.ParentClass.Name.Lexeme)
                {
                    throw new SemanticError(statement.Name, "Class can't inherit from itself");
                }
                _callingBodyStack.Push(CallingBody.SUBCLASS);
                parentClass = (HlangClassDeclaration)Resolve(statement.ParentClass);
                _env.Add("parent", parentClass);
            }


            foreach (Assign assignment in statement.Fields)
            {
                if (assignment.IsStatic)
                {
                    _env.Add(assignment.Name.Lexeme, Resolve(assignment.Value));
                }
                else
                {
                    newClass.Fields.Add(assignment.Name.Lexeme, Resolve(assignment.Value));

                }
            }

            foreach (Function method in statement.Methods)
            {
                if (method.IsStatic)
                {
                    var staticFunc = new HlangFunction(method, newClass.ClassEnv);
                    _env.Add(method.Name.Lexeme, staticFunc);
                }
                else
                {
                    var func = new HlangFunction(method, newClass.ClassEnv);
                    newClass.Methods.Add(method.Name.Lexeme, func);

                }
                if (method.Name.Lexeme == statement.Name.Lexeme)
                {
                    ResolveFunction(method, FunctionType.INIT, CallingBody.FUNCTION);
                }
                else
                {
                    ResolveFunction(method, FunctionType.METHOD, CallingBody.FUNCTION);
                }
            }

            EndScope();
            _callingBodyStack.Pop();
            newClass.ParentClass = parentClass;
            newClass.ClassEnv = _env;
            Define(statement.Name, newClass);
            return null;
        }

        public object VisitGetPropertyExpr(GetProperty expr)
        { 
            Resolve(expr.Object);
            return null;
        }

        public object VisitSetPropertyExpr(SetProperty expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Object);
            return null;
        }

        public object VisitThisExpr(This expr)
        {
            if (_callingBodyStack.Contains(CallingBody.CLASS) && _callingBodyStack.Contains(CallingBody.FUNCTION))
            {
                return null;
            }
            throw new SemanticError(expr.Keyword, "'this' must be used inside of a class");
        }

        public object VisitParentExpr(Parent expr)
        {
            var parent = (HlangClassDeclaration)GetValue(expr.Keyword);

            if (parent != null && parent is ICallable)
            {
                int expectedArguments = ((ICallable)parent).ArgumentLength;
                if (expectedArguments != expr.Arguments.Count)
                {
                    throw new SemanticError(expr.Keyword, $"Expected {expectedArguments} arguments but got {expr.Arguments.Count}");
                }
            }

            foreach (Expr argument in expr.Arguments)
            {
                Resolve(argument);
            }
            return null;
        }
    }
}
