using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Represent and execute an assignment expression
    /// </summary>
    public class Assign : Expr
    {
        public Token Name { get; set; }
        public Expr Value { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPrivate { get; set; }

        public Assign(Token name, Expr value, bool isStatic = false, bool isPrivate = false)
        {
            Name = name;
            Value = value;
            IsStatic = isStatic;
            IsPrivate = isPrivate;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }
}
