using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HelperInterfaces
{
    public interface ICallable
    {
        int ArgumentLength { get; set; }
        object Call(Interpreter interpreter, List<object> arguments); 
    }
}
