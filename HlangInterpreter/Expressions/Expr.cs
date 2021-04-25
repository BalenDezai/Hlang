using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Expressions
{
    /// <summary>
    /// Used to implement the visitor pattern and execution of expressions
    /// </summary>
    public abstract class Expr
    {
        public Token Name { get; set; }
        public Expr Left { get; set; }
        public Token Operator { get; set; }
        public Expr Right { get; set; }

        public abstract T Accept<T>(IExpressionVisitor<T> visitor);
    }
}
