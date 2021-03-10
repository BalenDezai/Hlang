using HlangInterpreter.TokenEnums;

namespace HlangInterpreter.lib
{
    public class Token
    {
        public string Lexeme { get; set; }
        public object Literal { get; set; }
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
