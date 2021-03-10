namespace HlangInterpreter.TokenEnums
{
    public enum TokenType
    {
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET,
        COMA, DOT,
        SUBTRACT, MINUS, MODULUS, ADD, PLUS, REVERSE,
        
        
        GREATER_EQUAL, LESS_EQUAL, IS_NOT, IS_EQUAL,

        INDENT, DEDENT,

        IDENTIFER, STRING, NUMBER, LIST,

        IS, NOT, EQUAL, GREATER, LESS, THAN, DEFINE,
        DIVIDE, MULTIPLY, BY, TO,

        AND, ELSE, FALSE, FUNCTION, FOR, EACH, IF, NOTHING, OR, IN,
        PRINT, RETURN, TRUE, WHILE, THEN, BREAK,

        EOF
    }
}
