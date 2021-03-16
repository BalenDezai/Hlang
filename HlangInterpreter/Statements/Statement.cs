using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    /// <summary>
    /// Parent class used to implement the visitor pattern
    /// </summary>
    public abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}
