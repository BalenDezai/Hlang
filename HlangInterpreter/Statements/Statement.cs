using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.Statements
{
    public abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}
