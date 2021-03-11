using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.Expressions
{
    public class Lambda : Expr
    {
        public List<Token> Parameters { get; set; }
        public Expr Body { get; set; }
        public Lambda(List<Token> parameters, Expr body)
        {
            Parameters = parameters;
            Body = body;
        }
        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLambdaExpr(this);
        }
    }
}
