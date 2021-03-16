using HlangInterpreter.Lib;
using System;

namespace HlangInterpreter.Errors
{
    /// <summary>
    /// Code runtime error exception
    /// </summary>
    public class RuntimeError : Exception
    {
        /// <summary>
        /// Token where error happened
        /// </summary>
        public Token Token { get; set; }
        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}
