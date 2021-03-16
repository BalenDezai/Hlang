using HlangInterpreter.Lib;
using System.Collections.Generic;

namespace HlangInterpreter.HelperInterfaces
{
    /// <summary>
    /// Interface foe function and lambda execution
    /// </summary>
    public interface ICallable
    {
        int ArgumentLength { get; set; }
        object Call(Interpreter interpreter, List<object> arguments); 
    }
}
