using HlangInterpreter.HelperInterfaces;
using HlangInterpreter.Lib;
using HlangInterpreter.Statements;
using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes
{
    class HlangFunction : ICallable
    {

        private int _argumentLength;
        public int ArgumentLength { get => _argumentLength; set => _argumentLength = value; }

        private Environment _closure;

        private Function _funcDeclaration;
        public HlangFunction(Function funcDeclaration, Environment closure)
        {
            _closure = closure;
            _funcDeclaration = funcDeclaration;
            ArgumentLength = funcDeclaration.Paramters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Lib.Environment  env = new Lib.Environment(_closure);
            for (int i = 0; i < _funcDeclaration.Paramters.Count; i++)
            {
                env.Add(_funcDeclaration.Paramters[i].Lexeme, arguments[i]);
            }
            try
            {
                interpreter.ExecuteBlock(_funcDeclaration.Body, env);
            }
            catch (HlangReturn value)
            {
                return value.Value;
            }
            return null;
        }

        public override string ToString()
        {
            return $"function {_funcDeclaration.Name.Lexeme}";
        }
    }
}
