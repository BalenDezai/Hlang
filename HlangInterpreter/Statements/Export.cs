using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    public class Export : Statement
    {
        public List<Expr> VarsToExport { get; set; }
        public Export(List<Expr> varsToExport)
        {
            VarsToExport = varsToExport;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitExportStatement(this);
        }
    }
}
