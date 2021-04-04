using HlangInterpreter.Lib;
using System;

namespace HlangInterpreter.Errors
{
    /// <summary>
    /// Code semantic analysis exception
    /// </summary>
    public class SemanticError : Exception
    {
        public Token Token { get; set; }
        public int Line { get; set; }
        public SemanticError(Token token, string message) : base (message)
        {
            Token = token;
            Line = token.Line;
        }
    }
}
