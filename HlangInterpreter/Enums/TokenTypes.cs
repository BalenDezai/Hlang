namespace HlangInterpreter.Enums
{
    public enum TokenType
    {
        // Types
        IDENTIFER, STRING, NUMBER, LIST,

        // Symbols
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET,
        COMA, DOT,

        // Unary
        NOT, TYPE, COMPLEMENT, INCREMENT, DECREMENT, OF,

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
        FUNCTION, FOR, EACH, IF, NOTHING, OR, IN, THIS,
        PRINT, RETURN, TRUE, WHILE, THEN, BREAK, CLASS,
        EXTENDS, STATIC, PARENT, PRIVATE,

        // end of file
        EOF
    }
}
