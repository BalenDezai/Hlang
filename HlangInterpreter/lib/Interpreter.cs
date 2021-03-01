using HlangInterpreter.ExprLib;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.TokenEnums;
using HlangInterpreter.Errors;
using System.Collections.Generic;
using HlangInterpreter.StmtLib;
using System;

namespace HlangInterpreter.lib
{
    public class Interpreter : IExprVisitor<object>, IStatementVisitor<object>
    {
        public Environment Environment { get; set; }
        public Interpreter()
        {
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

        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        private void Execute(Statement statement)
        {
            statement.Accept(this);
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
            throw new NotImplementedException();
        }
    }
}
