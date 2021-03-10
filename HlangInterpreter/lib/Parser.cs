using HlangInterpreter.TokenEnums;
using System.Collections.Generic;
using HlangInterpreter.Expressions;
using HlangInterpreter.Errors;
using HlangInterpreter.Statements;
using HlangInterpreter.HlangTypes;

namespace HlangInterpreter.lib
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _current = 0;
        private readonly ErrorReporting _errorReporting;

        public Parser(ErrorReporting errorReporting)
        {
            _errorReporting = errorReporting;
        }
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

        private Function FunctionStatement()
        {
            Consume(TokenType.FUNCTION, "Expect 'function' after 'define'");
            Token name = Consume(TokenType.IDENTIFER, "Expect function name");
            Consume(TokenType.LEFT_PAREN, "Expected '(' after function name");
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


        private Statement ForStatement()
        {
            Consume(TokenType.EACH, "Expect 'each' after for");
            Variable identifer = (Variable)Expression();
            Consume(TokenType.IN, "Expect 'in' after identifer");
            Expr list = Expression();
            Consume(TokenType.THEN, "Expect 'then' after list name");
            Block body = new Block(new List<Statement> { Statement() });
            return new ForEach(identifer, list, body);
        }

        private Statement WhileStatement()
        {
            Expr condition = Expression();
            Consume(TokenType.THEN, "Expect 'then' after while condition");
            Statement body = Statement();
            return new While(condition, body);
        }

        private Statement IfStatement()
        {
            Expr condition = Expression();
            Consume(TokenType.THEN, "Expect 'then' after if condition");

            Statement thenBranch = Statement();
            Statement elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                Consume(TokenType.THEN, "Expect 'then' after else");
                elseBranch = Statement();
            }
            return new If(condition, thenBranch, elseBranch);
        }

        private List<Statement> Block()
        {
            if (Previous().Type != TokenType.THEN)
            {
                throw new ParsingError(Peek(), "Unexpected use of indentation");
            }

            Advance();

            List<Statement> statements = new List<Statement>();

            while (!Match(TokenType.DEDENT) && !IsAtEnd())
            {
                statements.Add(Decalration());
            }

            return statements;
        }

        private Statement ReturnStatement()
        {
            Expr value = null;
            if (!Check(TokenType.DEDENT))
            {
                value = Expression();
            }
            return new Return(value);
        }

        private Statement BreakStatement()
        {
            return new Break(Previous());
        }

        private Statement ExpressionStatement()
        {
            Expr expr = Expression();
            return new Expression(expr);
        }

        private Statement PrintStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' for function call");
            Expr value = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' for function call");
            return new Print(value);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            Expr expr = Or();


            if (Check(TokenType.IS))
            {
                Token Name = Previous();
                Expr init = null;
                if (MatchNext(TokenType.EQUAL, TokenType.NOT)) return Equality();
                Advance();
                init = Assignment();
                return new Assign(Name, init);
            }
            
            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();
            while (Match(TokenType.OR))
            {
                Token opr = Previous();
                Expr right = And();
                expr = new Logical(expr, opr, right);
            }

            return expr;
        }

        private Expr And()
        {
            Expr expr = Equality();
            while(Match(TokenType.AND))
            {
                Token opr = Previous();
                Expr right = Equality();
                expr = new Logical(expr, opr, right);
            }
            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            if (Check(TokenType.IS))
            {
                while (MatchNext(TokenType.NOT, TokenType.EQUAL))
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

        private Expr Comparison()
        {
            Expr expr = Term();
            if (Check(TokenType.IS))
            {
                bool equal = false;
                // potential trouble
                if (MatchNextConsecutively(TokenType.EQUAL, TokenType.OR))
                {
                    equal = true;
                }

                while (MatchNext(TokenType.GREATER, TokenType.LESS))
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

        private Expr Unary()
        {
            if (Match(TokenType.NOT) || Match(TokenType.REVERSE))
            {
                Token opr = Previous();
                Expr right = Unary();
                return new Unary(opr, right);
            }
            return FunctionCall();
        }

        private Expr FunctionCall()
        {
            Expr expr = Primary();
            while (true)
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishFunctionCall(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expr Lambda()
        {

        }

        private Expr Primary()
        {

            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NOTHING)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFER)) return new Variable(Previous());

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

            if(Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Grouping(expr);
            }
            throw new ParsingError(Peek(), "Expected an expression");
        }

        private Expr FinishFunctionCall(Expr callee)
        {
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
                    arguments.Add(Expression());
                } while (Match(TokenType.COMA));
            }
            Token paren = Consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments");
            return new FunctionCall(callee, paren, arguments);
        }

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
                        return;
                }
                Advance();
            }

        }

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

        private bool MatchNextConsecutively(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (!CheckNext(type))
                {
                    return false;
                }
            }
            for (int i = 0; 0 < types.Length; i++)
            {
                Advance();
            }
            return true;
        }

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

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private bool CheckNext(TokenType type)
        {
            if (IsAtEnd()) return false;
            return PeekNext().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token PeekNext()
        {
            if (IsAtEnd()) return null;
            return _tokens[_current + 1];
        }
        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Consume(TokenType type, string msg)
        {
            if (Check(type)) return Advance();
            throw new ParsingError(Peek(), msg);
        }
    }
}
