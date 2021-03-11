namespace HlangInterpreter.TokenEnums
{
    public enum TokenType
    {
        // Types
        IDENTIFER, STRING, NUMBER, LIST,

        // Symbols
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET,
        COMA, DOT,

        // Unary
        REVERSE, NOT,

        // Arithmetic
        SUBTRACT, MINUS, MODULUS, ADD, PLUS,
        DIVIDE, MULTIPLY, BY,

        // Comparison
        GREATER_EQUAL, LESS_EQUAL, GREATER, LESS, THAN,

        // Equality
        IS_NOT, IS_EQUAL,

        // block indentation
        INDENT, DEDENT,

        // Expression and statements
        IS, EQUAL, DEFINE, TO, LAMBDA, AND, ELSE, FALSE,
        FUNCTION, FOR, EACH, IF, NOTHING, OR, IN,
        PRINT, RETURN, TRUE, WHILE, THEN, BREAK,

        // end of file
        EOF
    }
}
