using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.Statements
{
    public class Class : Statement
    {
        public List<Function> Methods { get; set; }
        public Variable ParentClass { get; set; }
        public List<Assign> Fields { get; set; }

        public Class(List<Function> methods, List<Assign> fields, Token name, Variable parentClass)
        {
            Methods = methods;
            Name = name;
            ParentClass = parentClass;
            Fields = fields;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitClassSTatement(this);
        }
    }
}
