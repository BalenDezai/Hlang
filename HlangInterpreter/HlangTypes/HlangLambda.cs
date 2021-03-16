using HlangInterpreter.Expressions;
using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes
{
    /// <summary>
    /// Represents a lambda in Hlang
    /// </summary>
    public class HlangLambda : ICallable
    {
        public int ArgumentLength { get; set; }
        public Lambda Expr { get; set; }
        public HlangLambda(Lambda expr)
        {
            Expr = expr;
            ArgumentLength = expr.Parameters.Count;
        }
        /// <summary>
        /// Function executed when the lambda is called in hlang
        /// </summary>
        /// <param name="interpreter">Interpreter to execute expression</param>
        /// <param name="arguments">Arguments passed in</param>
        /// <returns></returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            // keep track of the old environment
            // and create a new one for the lambda execution
            Lib.Environment old = interpreter.Environment;
            Lib.Environment env = new Lib.Environment(old);

            // add arguments passed in to new environment
            for (int i = 0; i < Expr.Parameters.Count; i++) {
                env.Add(Expr.Parameters[i].Lexeme, arguments[i]);
            }

            // evaluate expression and change interpeter environment back
            interpreter.Environment = env;
            object value = interpreter.Evaluate(Expr.Body);
            interpreter.Environment = old;
            return value;
        }
    }
}
