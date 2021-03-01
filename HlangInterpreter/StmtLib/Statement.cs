using HlangInterpreter.HelperInterfaces;

namespace HlangInterpreter.StmtLib
{
    public abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}
