namespace HlangInterpreter.TokenEnums
{
    public enum TokenType
    {
        LEFT_PAREN, RIGHT_PAREN, COMA, DOT, NEW_ENVIRONMENT,
        SUBTRACT, MODULUS, ADD, 
        
        
        //DIVIDE_BY, MULTIPLY_BY,


        IDENTIFER, STRING, NUMBER,


        //IS, NOT, NOT_EQUAL, IS_EQUAL, IS_GREATER_THAN, IS_LESS_THAN,
        //IS_GREATER_OR_EQUAL_TO, IS_LESSER_OR_EQUAL_TO,

        IS, NOT, EQUAL, GREATER, LESS, THAN, DEFINE,
        DIVIDE, MULTIPLY, BY, TO,

        AND, ELSE, FALSE, FUNCTION, FOR, EACH, IF, NOTHING, OR,
        PRINT, RETURN, TRUE, WHILE, THEN,

        EOF
    }
}
