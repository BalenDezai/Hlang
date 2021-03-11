using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes
{
    public class HlangLambda : ICallable
    {
        private int _argumentLength;
        public int ArgumentLength { get => _argumentLength; set => _argumentLength = value; }

        public Lambda Expr { get; set; }
        public HlangLambda(Lambda expr)
        {
            Expr = expr;
            ArgumentLength = expr.Parameters.Count;
        }
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Lib.Environment old = interpreter.Environment;
            Lib.Environment env = new Lib.Environment();
            for (int i = 0; i < Expr.Parameters.Count; i++) {
                env.Add(Expr.Parameters[i].Lexeme, arguments[i]);
            }
            interpreter.Environment = env;
            object value = interpreter.Evaluate(Expr.Body);
            interpreter.Environment = old;
            return value;
        }
    }
}
