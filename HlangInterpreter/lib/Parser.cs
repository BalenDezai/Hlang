using HlangInterpreter.TokenEnums;
using System.Collections.Generic;
using HlangInterpreter.Expressions;
using HlangInterpreter.Errors;
using HlangInterpreter.Statements;
using HlangInterpreter.HlangTypes;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// The parser that parses Hlang expressions and statements
    /// </summary>
    public class Parser
    {
        private List<Token> _tokens;
        private int _current = 0;
        private readonly ErrorReporting _errorReporting;

        public Parser(ErrorReporting errorReporting)
        {
            _errorReporting = errorReporting;
        }
        /// <summary>
        /// Parses the tokens into executable expressions and statements
        /// </summary>
        /// <param name="tokens">List of tokens to parse</param>
        /// <returns>A list of statements and expressions</returns>
        public List<Statement> Parse(List<Token> tokens)
        {
            _tokens = tokens;
            List<Statement> Statements = new List<Statement>();
            while (!IsAtEnd())
            {
                Statements.Add(Decalration());
            }
            return Statements;
        }

        /// <summary>
        /// Parses declarations
        /// </summary>
        private Statement Decalration()
        {
            try
            {
                if (Match(TokenType.DEFINE)) return FunctionStatement();
                return Statement();
            }
            catch (ParsingError err)
            {
                _errorReporting.ReportError(err.Token.Line, err.Token.Lexeme, err.Message);
                SynchronizeParsing();
                return null;
            }
            
        }
        /// <summary>
        /// Parses a 'define function' statement
        /// </summary>
        /// <returns>A function node</returns>
        private Function FunctionStatement()
        {
            // syntactic check and get the function name
            Consume(TokenType.FUNCTION, "Expect 'function' after 'define'");
            Token name = Consume(TokenType.IDENTIFER, "Expect function name");
            Consume(TokenType.LEFT_PAREN, "Expected '(' after function name");

            // get the parameter names while also making sure it's syntactically correct
            List<Token> paramters = new List<Token>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (paramters.Count >= 20)
                    {
                        Token token = Peek();
                        _errorReporting.ReportError(token.Line, token.Lexeme, "Can't have more than 20 parameters");
                    }
                    paramters.Add(Consume(TokenType.IDENTIFER, "Expect paramter name"));
                } while (Match(TokenType.COMA));
            }

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after paramters");
            Consume(TokenType.THEN, "Expected 'then' after function declaration");
            List<Statement> body = Block();

            return new Function(name, paramters, body);
        }

        /// <summary>
        /// Parses a statement
        /// </summary>
        /// <returns>A statement node</returns>
        private Statement Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.FOR)) return ForStatement();
            if (Check(TokenType.INDENT)) return new Block(Block());
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.BREAK)) return BreakStatement();
            return ExpressionStatement();
        }

        /// <summary>
        /// Parse a 'for each' statement
        /// </summary>
        /// <returns>A 'for each' statement node</returns>
        private Statement ForStatement()
        {
            // syntactic check and get the identifier name
            Consume(TokenType.EACH, "Expect 'each' after for");
            Variable identifer = (Variable)Expression();
            Consume(TokenType.IN, "Expect 'in' after identifer");

            // get the list to iterate
            Expr list = Expression();
            Consume(TokenType.THEN, "Expect 'then' after list name");

            // get the body statement to execute
            Block body = new Block(new List<Statement> { Statement() });

            return new ForEach(identifer, list, body);
        }
        /// <summary>
        /// Parse a 'while' statement
        /// </summary>
        /// <returns>A 'while' statement node</returns>
        private Statement WhileStatement()
        {
            // get the condition expression
            Expr condition = Expression();
            Consume(TokenType.THEN, "Expect 'then' after while condition");

            // get the  'while' statement body
            Statement body = Statement();
            return new While(condition, body);
        }

        /// <summary>
        /// Parse an 'if' statement
        /// </summary>
        /// <returns>A 'if' statement node</returns>
        private Statement IfStatement()
        {
            Expr condition = Expression();
            Consume(TokenType.THEN, "Expect 'then' after if condition");

            // get the block to execute if and the else branch if it exists
            Statement thenBranch = Statement();
            Statement elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                Consume(TokenType.THEN, "Expect 'then' after else");
                elseBranch = Statement();
            }
            return new If(condition, thenBranch, elseBranch);
        }
        /// <summary>
        /// Parse an indentation block
        /// </summary>
        /// <returns>A block statement node</returns>
        private List<Statement> Block()
        {
            // blocks are not allowed unless after a 'then' keyword
            if (Previous().Type != TokenType.THEN)
            {
                throw new ParsingError(Peek(), "Unexpected use of indentation");
            }

            // consume the indent and keep reading statements until we meet the dedent token
            Consume(TokenType.INDENT, "Expected indentation after 'then'");

            List<Statement> statements = new List<Statement>();

            while (!Match(TokenType.DEDENT) && !IsAtEnd())
            {
                statements.Add(Decalration());
            }

            return statements;
        }
        /// <summary>
        /// Parse a 'return' statement
        /// </summary>
        /// <returns>A 'return' statement node</returns>
        private Statement ReturnStatement()
        {
            Expr value = null;
            // check if a value is returned or its void
            if (!Check(TokenType.DEDENT))
            {
                value = Expression();
            }
            return new Return(value);
        }

        /// <summary>
        /// Parse a 'break' statement
        /// </summary>
        /// <returns>A 'break' statement node</returns>
        private Statement BreakStatement()
        {
            return new Break(Previous());
        }

        /// <summary>
        /// Parse an expression statement
        /// </summary>
        /// <returns>An expression statement node</returns>
        private Statement ExpressionStatement()
        {
            return new Expression(Expression());
        }

        /// <summary>
        /// Parse a 'print()' statement
        /// </summary>
        /// <returns>A print statement node</returns>
        private Statement PrintStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' for function call");
            Expr value = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' for function call");
            return new Print(value);
        }

        /// <summary>
        /// Parse an expression
        /// </summary>
        /// <returns>An expression node</returns>
        private Expr Expression()
        {
            return Assignment();
        }
        /// <summary>
        /// Parse an assignment expression
        /// </summary>
        /// <returns>An assignment expression node or an expression node<</returns>
        private Expr Assignment()
        {
            Expr expr = Or();

            if (Check(TokenType.IS))
            {
                // If it's not an assignment, parse equality instead
                if (MatchNext(TokenType.EQUAL, TokenType.NOT)) return Equality();

                // get the variable identifier
                Token Name = Previous();
                
                Advance();
                // recursively get the expression to assign
                Expr init = Assignment();

                return new Assign(Name, init);
            }
            
            return expr;
        }
        /// <summary>
        /// Parse a logical 'or' expression
        /// </summary>
        /// <returns>A logical 'or' expression node or an expression node<</returns>
        private Expr Or()
        {
            Expr expr = And();
            if (Match(TokenType.OR))
            {
                Token opr = Previous();
                Expr right = And();
                expr = new Logical(expr, opr, right);
            }

            return expr;
        }
        /// <summary>
        /// Parse a logical 'and' expression
        /// </summary>
        /// <returns>A logical 'and' expression node or an expression node</returns>
        private Expr And()
        {
            Expr expr = Equality();

            if(Match(TokenType.AND))
            {
                Token opr = Previous();
                Expr right = Equality();
                expr = new Logical(expr, opr, right);
            }
            return expr;
        }
        /// <summary>
        /// Parse an equality expression
        /// </summary>
        /// <returns>A binary equality expression node or an expression node</returns>
        private Expr Equality()
        {
            Expr expr = Comparison();

            if (Check(TokenType.IS))
            {
                if (MatchNext(TokenType.NOT, TokenType.EQUAL))
                {
                    Token opr = Previous();

                    if (opr.Type == TokenType.EQUAL)
                    {
                        Consume(TokenType.TO, "Expect 'to' after expression");
                    }
                    
                    Expr right = Comparison();
                    expr = new Binary(expr, opr, right);
                }
            }

            return expr;
        }
        /// <summary>
        /// Parse a comparison expression
        /// </summary>
        /// <returns>A binary comparison expression node or an expression node</returns>
        private Expr Comparison()
        {
            Expr expr = Term();
            if (Check(TokenType.IS))
            {
                bool equal = false;
                
                // Check if it is actually a comparison expression
                if (CheckNext(TokenType.EQUAL) && CheckNextTwo(TokenType.OR))
                {
                    // advance the two checked tokens
                    Advance();
                    Advance();
                    equal = true;
                }
                
                // find out which kind of comparison it is then return the node
                if (MatchNext(TokenType.GREATER, TokenType.LESS))
                {
                    Token opr = Previous();
                    if (opr.Type == TokenType.GREATER && equal)
                    {
                        opr.Type = TokenType.GREATER_EQUAL;
                        
                    }
                    else if (opr.Type == TokenType.GREATER)
                    {
                        opr.Type = TokenType.GREATER;
                    }
                    else if (opr.Type == TokenType.LESS && equal)
                    {
                        opr.Type = TokenType.LESS_EQUAL;
                    }
                    else
                    {
                        opr.Type = TokenType.LESS;
                    }
                    Consume(TokenType.THAN, "Expect 'than' after expression");

                    Expr right = Primary();
                    expr = new Binary(expr, opr, right);
                }
            }
            return expr;
        }
        /// <summary>
        /// Parse a term expression
        /// </summary>
        /// <returns>A binary term expression node or an expression node</returns>
        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.ADD, TokenType.SUBTRACT, TokenType.PLUS, TokenType.MINUS, TokenType.MODULUS))
            {
                Token opr = Previous();
                Expr right = Factor();
                expr = new Binary(expr, opr, right);
            }

            return expr;

        }
        /// <summary>
        /// Parse a factor expression
        /// </summary>
        /// <returns>A binary factoring expression node or an expression node</returns>
        private Expr Factor()
        {
            Expr expr = Unary();
            while (Match(TokenType.DIVIDE, TokenType.MULTIPLY))
            {
                Token opr = Previous();
                Consume(TokenType.BY, "Expect 'or' after expression");
                Expr right = Primary();
                expr = new Binary(expr, opr, right);

            }
            return expr;
        }
        /// <summary>
        /// Parse an unary expression
        /// </summary>
        /// <returns>An unary expression node or an expression node</returns>
        private Expr Unary()
        {
            if (Match(TokenType.NOT, TokenType.MINUS, TokenType.INCREMENT, TokenType.DECREMENT, TokenType.COMPLEMENT, TokenType.TYPE))
            {
                Token opr = Previous();
                if (opr.Type == TokenType.COMPLEMENT || opr.Type == TokenType.TYPE) Consume(TokenType.OF, "Expected 'of' after 'complement' or 'type'");
                Expr right = Unary();
                return new Unary(opr, right: right);
            }
            return Lambda();
        }
        /// <summary>
        /// Parse a lambda expression
        /// </summary>
        /// <returns>A lambda expression node or an expression node</returns>
        private Expr Lambda()
        {
            if (Match(TokenType.LAMBDA))
            {
                // parse the paramters of the lambda
                List<Token> paramters = new List<Token>();
                do
                {
                    //  only allow 5 paramters for a lambda
                    if (paramters.Count >= 5)
                    {
                        Token token = Peek();
                        _errorReporting.ReportError(token.Line, token.Lexeme, "Can't have more than 20 arguments");
                    }
                    paramters.Add(Consume(TokenType.IDENTIFER, "Expected paramter name"));
                } while (Match(TokenType.COMA));

                Consume(TokenType.THEN, "Expected 'then' after paramters");

                Expr body = Expression();
                return new Lambda(paramters, body);
            }
            return FunctionCall();
        }
        /// <summary>
        /// Parse a function call expression
        /// </summary>
        /// <returns>A function call expression node or an expression node</returns>
        private Expr FunctionCall()
        {
            Expr expr = Primary();
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishFunctionCall(expr);
            }
            return expr;
        }
        /// <summary>
        /// Parse a primary expression
        /// </summary>
        /// <returns>A primary expression node</returns>
        private Expr Primary()
        {
            // return string, number, bool, or 'nothing' literal
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NOTHING)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            // return identifier or grouping
            if (Match(TokenType.IDENTIFER)) return new Variable(Previous());

            if(Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Grouping(expr);
            }

            // return list literal
            if (Match(TokenType.LEFT_BRACKET))
            {
                HlangList<Expr> values = new HlangList<Expr>();
                while (!Match(TokenType.RIGHT_BRACKET))
                {
                    if (!Check(TokenType.COMA))
                    {
                        values.Add(Primary());
                    }
                    else
                    {
                        Advance();
                    }
                }
                return new List(values);
            }

            throw new ParsingError(Peek(), "Expected an expression");
        }
        /// <summary>
        /// Finishes the parsing of a function call expression
        /// </summary>
        /// <param name="callee">the identifer of the function</param>
        /// <returns>A function call expression node</returns>
        private Expr FinishFunctionCall(Expr callee)
        {
            // get the arguments
            List<Expr> arguments = new List<Expr>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 20)
                    {
                        Token token = Peek();
                        _errorReporting.ReportError(token.Line, token.Lexeme, "Can't have more than 20 arguments");
                    }
                    // make sure the arguments are parsed as well
                    // in case of for example funcCall(2 + 2)
                    arguments.Add(Expression());
                } while (Match(TokenType.COMA));
            }
            Token paren = Consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments");
            return new FunctionCall(callee, paren, arguments);
        }
        /// <summary>
        /// Will synchronize the parsing after it encounters an error.
        /// This is essential to find out further errors without causing cascading errrors
        /// </summary>
        public void SynchronizeParsing()
        {
            Advance();
            while (!IsAtEnd())
            {
                switch (Previous().Type)
                {

                    case TokenType.IDENTIFER:
                    case TokenType.DEFINE:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                    case TokenType.WHILE:
                    case TokenType.LAMBDA:
                        return;
                }
                Advance();
            }

        }
        /// <summary>
        /// Check if the current token matches any of the given tokens
        /// </summary>
        /// <param name="types">Tokens to match</param>
        private bool Match(params TokenType[] types)
        {
            foreach (var tokentype in types)
            {
                if (Check(tokentype))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Check if the next token matches any of the given tokens
        /// </summary>
        /// <param name="types">Tokens to match</param>
        /// <returns></returns>
        private bool MatchNext(params TokenType[] types)
        {
            foreach (var tokentype in types)
            {
                if (CheckNext(tokentype))
                {
                    Advance();
                    Advance();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the current token matches a type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns></returns>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }
        /// <summary>
        /// Check if the next token matches a type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        private bool CheckNext(TokenType type)
        {
            if (IsAtEnd()) return false;
            return PeekNext().Type == type;
        }
        /// <summary>
        /// Check if 2 tokens ahead of current token matches a type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        private bool CheckNextTwo(TokenType type)
        {
            if (IsAtEnd()) return false;
            return PeekNextTwo().Type == type;
        }
        /// <summary>
        /// Advance to the next token
        /// </summary>
        /// <returns>Previous token</returns>
        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }
        /// <summary>
        /// Check if at the end of token list
        /// </summary>
        /// <returns></returns>
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }
        /// <summary>
        /// Peek at the current token
        /// </summary>
        /// <returns>Current token</returns>
        private Token Peek()
        {
            return _tokens[_current];
        }
        /// <summary>
        /// Peek at the next token
        /// </summary>
        /// <returns>Next token</returns>
        private Token PeekNext()
        {
            if (IsAtEnd()) return null;
            return _tokens[_current + 1];
        }
        /// <summary>
        /// Peek at the token 2 tokens ahead
        /// </summary>
        /// <returns>Token that is 2 tokens ahead</returns>
        private Token PeekNextTwo()
        {
            if (IsAtEnd()) return null;
            return _tokens[_current + 2];
        }
        /// <summary>
        /// Get previous token
        /// </summary>
        /// <returns>Previous token</returns>
        private Token Previous()
        {
            return _tokens[_current - 1];
        }
        /// <summary>
        /// Consumes the current token of a certain type.
        /// Throws an error if it's not of the correct type
        /// </summary>
        /// <param name="type">Type to consume</param>
        /// <param name="msg">Error message</param>
        /// <returns>Consumed token</returns>
        private Token Consume(TokenType type, string msg)
        {
            if (Check(type)) return Advance();
            throw new ParsingError(Peek(), msg);
        }
    }
}
