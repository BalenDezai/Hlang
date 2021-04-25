using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    public class Import : Statement
    {
        public Expr FileName { get; set; }
        public List<Expr> Identifiers { get; set; }
        public Import(Expr fileName, List<Expr> identifiers = null)
        {
            FileName = fileName;
            Identifiers = identifiers;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitImportStatement(this);
        }
    }
}
