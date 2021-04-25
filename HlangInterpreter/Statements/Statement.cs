using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Parent class used to implement the visitor pattern
    /// </summary>
    public abstract class Statement
    {
        public Token Name { get; set; }
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}
