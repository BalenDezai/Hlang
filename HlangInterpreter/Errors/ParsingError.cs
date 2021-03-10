using HlangInterpreter.lib;
using System;
using HlangInterpreter.TokenEnums;

namespace HlangInterpreter.Errors
{
    public class ParsingError : Exception
    {
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
