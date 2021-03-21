﻿using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.TokenEnums;
using HlangInterpreter.Errors;
using System.Collections.Generic;
using HlangInterpreter.Statements;
using System;
using HlangInterpreter.HlangTypes;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Interpeter class that evaluates and executes statements and expressions
    /// </summary>
    public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        // Global functions and values
        public Environment Globals { get; set; }
        // The enviroment of the execution
        public Environment Environment { get; set; }
        public Interpreter()
        {
            Globals = new Environment();
            Environment = new Environment();
        }
        /// <summary>
        /// Interpret a list of statements
        /// </summary>
        /// <param name="statements">Statements to evaluate and execute</param>
        public void Interpret(List<Statement> statements)
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        /// <summary>
        /// Evaluate and execute the print statement
        /// </summary>
        /// <param name="statement">Statement to print</param>
        public object VisitPrintStatement(Print statement)
        {
            // evaluate the expression to be printed
            object value = Evaluate(statement.Expression);
            // print evaluation
            Console.WriteLine(Stringify(value));
            return null;
        }
        /// <summary>
        /// Evaluate and assign an expression
        /// </summary>
        /// <param name="expr">Assignment expression to evaluate and assign</param>
        public object VisitAssignExpr(Assign expr)
        {
            // evaluate the expression
            object value = Evaluate(expr.Value);
            // if the variable exists, reassign
            // otherwise create new variable
            if (Environment.VariableExists(expr.Name.Lexeme))
            {
                Environment.Assign(expr.Name, value);
            }
            else
            {
                Environment.Add(expr.Name.Lexeme, value);
            }
            return null;
        }
        /// <summary>
        /// Evaluate and execute a binary expression
        /// </summary>
        /// <param name="expr">Binary expression to evaluate and execute</param>
        /// <returns>Evaluated binary expression</returns>
        public object VisitBinaryExpr(Binary expr)
        {
            // evaluate the left and the right side
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
                // be sure to handle string concatination too
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
                    if (left is string && right != null)
                    {
                        return (string)left + (string)right.ToString();
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
        /// <summary>
        /// Evaluate and return a grouping expression
        /// </summary>
        /// <param name="expr">Grouping expression to evaluate</param>
        /// <returns>Evaluated grouping</returns>
        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }
        /// <summary>
        /// Evaluate a literal expression
        /// </summary>
        /// <param name="expr">Literal expression to evaluate</param>
        /// <returns>Evaluated literal expression</returns>
        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }
        /// <summary>
        /// Evaluate an unary expression
        /// </summary>
        /// <param name="expr">Unary expression</param>
        /// <returns>Evaluated unary expression</returns>
        public object VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.Right);
            switch (expr.Operator.Type)
            {
                case TokenType.NOT:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
                case TokenType.TYPE:
                    switch (right)
                    {
                        case int c:  return "integer";
                        case double c: return "float";
                        case string s: return "string";
                        case bool b: return "bool";
                        case HlangFunction f: return "function";
                        case HlangList<object> l: return "list";
                        case null: return "nothing";
                    }
                    throw new RuntimeError(expr.Operator, $"undefined type of variable: '{right}'");
                case TokenType.INCREMENT: 
                    var toIncrement = (Variable)expr.Right;
                    if (Environment.Values.ContainsKey(toIncrement.Name.Lexeme))
                    {
                        return Environment.Values[toIncrement.Name.Lexeme] = (double)right + 1;
                    }
                    throw new RuntimeError(expr.Operator, $"incrementing undefined variable: '{toIncrement.Name.Lexeme}'");
                case TokenType.DECREMENT:
                    var toDecrement = (Variable)expr.Right;
                    if (Environment.Values.ContainsKey(toDecrement.Name.Lexeme))
                    {
                        return Environment.Values[toDecrement.Name.Lexeme] = (double)right - 1;
                    }
                    throw new RuntimeError(expr.Operator, $"Decrementing undefined variable: '{toDecrement.Name.Lexeme}'");
                case TokenType.COMPLEMENT:
                    if (right is double)
                    {
                        throw new RuntimeError(expr.Operator, $"Bad operand for unary 'complement of': 'float'");
                    }
                    return ~(int)right;
            }
            return null;
        }

        /// <summary>
        /// Evaluate the variable expression
        /// </summary>
        /// <param name="expr">Variable expression to evaluate</param>
        /// <returns>The evaluated variable</returns>
        public object VisitVariableExpr(Variable expr)
        {
            return Environment.GetValue(expr.Name);
        }
        /// <summary>
        /// Evaluate an expression
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>Evaluated expression</returns>
        public object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }
        /// <summary>
        /// Execute a statement
        /// </summary>
        /// <param name="statement">Statement to execute</param>
        public void Execute(Statement statement)
        {
            statement.Accept(this);
        }
        /// <summary>
        /// Check if an operand is a number
        /// </summary>
        /// <param name="opr">Operator to throw error for</param>
        /// <param name="operand">Operand to check</param>
        private void CheckNumberOperand(Token opr, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(opr, "Operand must be a number");
        }
        /// <summary>
        /// Check if both operands are numbers
        /// </summary>
        /// <param name="opr">Operator to throw an error for</param>
        /// <param name="left">Left Operand to check</param>
        /// <param name="right">Right operand to check</param>
        private void CheckNumberOperands(Token opr, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(opr, "Operands must be a number");
        }
        /// <summary>
        /// Check the truthy of an object
        /// </summary>
        /// <param name="obj">Object to check truthyness of</param>
        /// <returns>Object's truthy or falsy-ness</returns>
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }
        /// <summary>
        /// Check if two objects are equal
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        private bool IsEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;
            // use C#'s built in equality check
            return left.Equals(right);
        }
        /// <summary>
        /// Stringify an object for printing purposes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Stringified object</returns>
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
        /// <summary>
        /// Execute an expression stateement
        /// </summary>
        /// <param name="statement">Expression statement to execute</param>
        public object VisitExpressionStatement(Expression statement)
        {
            return Evaluate(statement.Expr);
        }
        /// <summary>
        /// Execute an indentation block statement
        /// </summary>
        /// <param name="statement">Indentation block statement to execute</param>
        public object VisitBlockStatement(Block statement)
        {
            ExecuteBlock(statement.Statements, new Environment(Environment));
            return null;
        }
        /// <summary>
        /// Execute a block
        /// </summary>
        /// <param name="statements">Block statement to execute</param>
        /// <param name="environment">The environment of the block statement</param>
        public void ExecuteBlock(List<Statement> statements, Environment environment)
        {
            // store previous environment to resume once block execution is over
            Environment previous = Environment;
            try
            {
                Environment = environment;
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                Environment = previous;
            }
        }
        /// <summary>
        /// Execute an if statement
        /// </summary>
        /// <param name="statement">If statement to execute</param>
        public object VisitIfStatement(If statement)
        {
            // evalaute the condition expression and it's truthyness
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                Execute(statement.ThenBranch);
            }
            else if (statement.ElseBranch != null)
            {
                Execute(statement.ElseBranch);
            }
            return null;
        }
        /// <summary>
        /// Evaluate a logical expression
        /// </summary>
        /// <param name="expr">Logical expression to evaluate</param>
        /// <returns>Evaluated logical expressed</returns>
        public object VisitLogicalExpr(Logical expr)
        {
            object left = Evaluate(expr.Left);
            if (expr.Operator.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return true;
            }
            else
            {
                // if it's an and operator and left side is false
                // return false without checking right side
                if (!IsTruthy(left)) return false;
            }

            return Evaluate(expr.Right);
        }
        /// <summary>
        /// Execute a while statement
        /// </summary>
        /// <param name="statement">While statement to execute</param>
        public object VisitWhileStatement(While statement)
        {
            try
            {
                while (IsTruthy(Evaluate(statement.Condition)))
                {
                    Execute(statement.Body);

                }
            }
            catch (HlangBreak)
            {
                return null;
            }
            return null;
        }
        /// <summary>
        /// Evaluate a list expression
        /// </summary>
        /// <param name="expr">List expression to evaluate</param>
        /// <returns>Evaluated list</returns>
        public object VisitListExpr(List expr)
        {
            HlangList<object> values = new HlangList<object>();
            foreach (var item in expr.Values)
            {
                values.Add(Evaluate(item));
            }
            return values;
        }
        /// <summary>
        /// Execute a for each statement
        /// </summary>
        /// <param name="statement">For each statement to be executed</param>
        public object VisitForeachStatement(ForEach statement)
        {
            // get the list from the for eeach statement
            var list = (List<object>)Evaluate(statement.List);
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    // create new environment with old (current) one as parent
                    // add the identiifer to it and execute the block
                    Environment env = new Environment(Environment);
                    env.Add(statement.Identifier.Name.Lexeme, list[i]);
                    ExecuteBlock(statement.Block.Statements, env);
                    // re-assign the new value if it's changed
                    list[i] = env.GetValue(statement.Identifier.Name);
                }
            }
            catch (HlangBreak)
            {
                return null;
            }

            return null;
        }
        /// <summary>
        /// Evaluate a function call expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitFunctionCallExpr(FunctionCall expr)
        {
            // get the called function
            object callee = Evaluate(expr.Callee);
            // evaluate the passed arguments
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
            // if the passed arguments are less than or exceep function argument length
            if (arguments.Count != function.ArgumentLength)
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.ArgumentLength} arguments but got {arguments.Count}");
            }
            // call the function
            return function.Call(this, arguments);
        }
        /// <summary>
        /// Execute new function statement. Adds function to environment
        /// </summary>
        /// <param name="statement">Function statement to add</param>
        public object visitFunctionStatement(Function statement)
        {
            HlangFunction function = new HlangFunction(statement, Environment);
            Environment.Add(statement.Name.Lexeme, function);
            return null;
        }
        /// <summary>
        /// Execute a return statement
        /// </summary>
        /// <param name="statement">Return statement to execute</param>
        public object VisitReturnStatement(Return statement)
        {
            object value = null;
            if (statement.Value != null) value = Evaluate(statement.Value);
            // when met with a retun statement, throw error to be caught by caller
            throw new HlangReturn(value);
        }
        /// <summary>
        /// Execute a break statement
        /// </summary>
        /// <param name="statement">Break statement to execute</param>
        /// <returns>The break statement</returns>
        public object VisitBreakStatement(Break statement)
        {
            throw new HlangBreak();
        }
        /// <summary>
        /// Evaluate a lambda expression
        /// </summary>
        /// <param name="expr">Lambda expression to evaluate</param>
        /// <returns>Hlang Lambda type</returns>
        public object VisitLambdaExpr(Lambda expr)
        {
            return new HlangLambda(expr);
        }
    }
}
