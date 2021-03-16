using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represent and execute a function call expression
    /// </summary>
    public class FunctionCall : Expr
    {
        public Expr Callee { get; set; }
        public Token Paren { get; set; }
        public List<Expr> Arguments { get; set; }
        public FunctionCall(Expr callee, Token paren, List<Expr> arguments)
        {
            Callee = callee;
            Paren = paren;
            Arguments = arguments;
        }
        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitFunctionCallExpr(this);
        }
    }
}
