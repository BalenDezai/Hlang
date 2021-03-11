using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.TokenEnums;
using HlangInterpreter.Errors;
using System.Collections.Generic;
using HlangInterpreter.Statements;
using System;
using HlangInterpreter.HlangTypes;

namespace HlangInterpreter.Lib
{
    public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        public Environment Globals { get; set; }
        public Environment Environment { get; set; }
        public Interpreter()
        {
            Globals = new Environment();
            Environment = new Environment();
        }
        public void Interpret(List<Statement> statements)
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        public object VisitPrintStatement(Print statement)
        {
            object value = Evaluate(statement.Expression);
            Console.WriteLine(Stringify(value));
            return null;
        }
        public object VisitAssignExpr(Assign expr)
        {
            object value = Evaluate(expr.Value);
            //if (((Literal)expr.Value).Value != null)
            //{
            //    value = Evaluate(expr.Value);
            //}
            if (Environment.VariableExists(expr.Name.Lexeme))
            {
                Environment.Assign(expr.Name, value);
            }
            else
            {
                Environment.Add(expr.Name.Lexeme, value);
            }
            return value;
        }

        public object VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);
            switch (expr.Operator.Type)
            {
                case TokenType.NOT:
                    return !IsEqual(left, right);
                case TokenType.EQUAL:
                    return IsEqual(left, right);
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.PLUS:
                case TokenType.ADD:
                    if (left is double  && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings");
                case TokenType.MINUS:
                case TokenType.SUBTRACT:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.MODULUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left % (double)right;
                case TokenType.MULTIPLY:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.DIVIDE:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
            }
            return null;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.Right);
            switch (expr.Operator.Type)
            {
                case TokenType.NOT:
                    return !IsTruthy(right);
                case TokenType.REVERSE:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }
            return null;
        }

        public object VisitVariableExpr(Variable expr)
        {
            return Environment.GetValue(expr.Name);
        }

        public object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        public object Execute(Statement statement)
        {
            var test = statement.Accept(this);
            return test;
        }

        private void CheckNumberOperand(Token opr, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(opr, "Operand must be a number");
        }

        private void CheckNumberOperands(Token opr, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(opr, "Operands must be a number");
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;
            return left.Equals(right);
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nothing";
            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return obj.ToString();
        }

        public object VisitExpressionStatement(Expression statement)
        {
            Evaluate(statement.Expr);
            return null;
        }

        public object VisitBlockStatement(Block statement)
        {
            return ExecuteBlock(statement.Statements, new Environment(Environment));
        }

        public object ExecuteBlock(List<Statement> statements, Environment environment)
        {
            Environment previous = Environment;
            object value = null;
            try
            {
                Environment = environment;
                foreach (var statement in statements)
                {
                    value = Execute(statement);
                }
            }
            finally
            {
                Environment = previous;
            }
            return value;
        }

        public object VisitIfStatement(If statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                return Execute(statement.ThenBranch);
            } else if (statement.ElseBranch != null)
            {
                return Execute(statement.ElseBranch);
            }
            return null;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            object left = Evaluate(expr.Left);
            if (expr.Operator.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return true;
            }
            else
            {
                if (!IsTruthy(left)) return false;
            }

            return Evaluate(expr.Right);
        }

        public object VisitWhileStatement(While statement)
        {
            while(IsTruthy(Evaluate(statement.Condition)))
            {
                var ret = Execute(statement.Body);
                if (ret is Break)
                {
                    break;
                }
                
            }
            return null;
        }

        public object VisitListExpr(List expr)
        {
            HlangList<object> values = new HlangList<object>();
            foreach (var item in expr.Values)
            {
                values.Add(Evaluate(item));
            }
            return values;
        }

        public object VisitForeachStatement(ForEach statement)
        {
            var list = (List<object>)Evaluate(statement.List);
            for (int i = 0; i < list.Count; i++)
            {
                Environment env = new Environment(Environment);
                env.Add(statement.Identifier.Name.Lexeme, list[i]);
                if (ExecuteBlock(statement.Block.Statements, env) is Break)
                {
                    break;
                }
                list[i] = env.GetValue(statement.Identifier.Name);
            }
            return null;
        }

        public object VisitFunctionCallExpr(FunctionCall expr)
        {
            object callee = Evaluate(expr.Callee);
            List<object> arguments = new List<object>();
            foreach (Expr argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeError(expr.Paren, "Only functions are callable");
            }

            ICallable function = (ICallable)callee;
            if (arguments.Count != function.ArgumentLength)
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.ArgumentLength} arguments but got {arguments.Count}");
            }
            return function.Call(this, arguments);
        }

        public object visitFunctionStatement(Function statement)
        {
            HlangFunction function = new HlangFunction(statement, Environment);
            Environment.Add(statement.Name.Lexeme, function);
            return null;
        }

        public object VisitReturnStatement(Return statement)
        {
            object value = null;
            if (statement.Value != null) value = Evaluate(statement.Value);
            throw new HlangReturn(value);
        }

        public object VisitBreakStatement(Break statement)
        {
            return statement;
        }

        public object VisitLambdaExpr(Lambda expr)
        {
            return new HlangLambda(expr);
        }
    }
}
