using HlangInterpreter.Lib;
using System;
using HlangInterpreter.TokenEnums;

namespace HlangInterpreter.Errors
{
    /// <summary>
    /// Code parsing error exception
    /// </summary>
    public class ParsingError : Exception
    {
        /// <summary>
        /// Token where the error happened
        /// </summary>
        public Token Token { get; set; }
        public ParsingError(Token token, string message) : base(message)
        {
            if (token.Type == TokenType.EOF)
            {
                token.Lexeme = "end";
            }
            Token = token;
        }
    }
}
