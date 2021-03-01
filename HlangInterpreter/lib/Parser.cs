using HlangInterpreter.TokenEnums;
using System.Collections.Generic;
using HlangInterpreter.ExprLib;
using HlangInterpreter.Errors;
using HlangInterpreter.StmtLib;

namespace HlangInterpreter.lib
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _current = 0;

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
            return Statement();
        }

        private Statement Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Check(TokenType.INDENT)) return new Block(Block());
            return ExpressionStatement();
        }

        private List<Statement> Block()
        {
            if (Previous().Type != TokenType.THEN)
            {
                throw new ParsingError(Peek(), "Unexpected use of indentation");
            }

            List<Statement> statements = new List<Statement>();
            return statements;
        }

        private Statement ExpressionStatement()
        {
            Expr expr = Expression();
            return new Expression(expr);
        }

        private Statement PrintStatement()
        {
            Expr value = Expression();
            return new Print(value);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Equality();

            //if (Match(TokenType.IS))
            //{
            //    Token isToken = Previous();
            //    Expr value = Assignment();

            //    if (expr is Variable)
            //    {
            //        Token name = ((Variable)expr).Name;
            //        return new Assign(name, value);
            //    }
            //    throw new ParsingError(isToken, "Assignment target is invalid");
            //}

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

        private Expr Equality()
        {
            Expr expr = Comparison();

            if (Check(TokenType.IS))
            {
                while (MatchNext(TokenType.NOT, TokenType.EQUAL))
                {
                    if (Previous().Type == TokenType.EQUAL)
                    {
                        Consume(TokenType.TO, "Expect 'to' after expression");
                    }
                    Token opr = Peek();
                    
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

                while (Match(TokenType.GREATER, TokenType.LESS))
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

            while (Match(TokenType.ADD, TokenType.SUBTRACT, TokenType.PLUS, TokenType.MINUS))
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
            return Primary();
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

            if(Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Grouping(expr);
            }
            throw new ParsingError(Peek(), "Expected an expression");
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
                if (CheckNext(type))
                {
                    Advance();
                } else
                {
                    return false;
                }
            }
            Advance();
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
