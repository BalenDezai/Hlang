using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.Expressions
{
    public class Parent : Expr
    {
        public Token Keyword { get; set; }
        public List<Expr> Arguments { get; set; }
        public Parent(Token keyword, List<Expr> arguments)
        {
            Keyword = keyword;
            Arguments = arguments;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitParentExpr(this);
        }
    }
}
