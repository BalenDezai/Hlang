using HlangInterpreter.Enums;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Token class to extra tokens out of code sequences
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The token key word
        /// </summary>
        public string Lexeme { get; set; }
        /// <summary>
        /// The token literal value
        /// </summary>
        public object Literal { get; set; }
        /// <summary>
        /// Line where token is
        /// </summary>
        public int Line { get; set; }
        public TokenType Type { get; set; }
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
