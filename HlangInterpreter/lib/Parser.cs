using HlangInterpreter.TokenEnums;
using System.Collections.Generic;
using HlangInterpreter.ExprLib;
using System;

namespace HlangInterpreter.lib
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Expr> Parse()
        {
            List<Expr> expressions = new List<Expr>();
            while (!IsAtEnd())
            {
                expressions.Add(Expression());
            }
            return expressions;
        }

        private Expr Expression()
        {
            var x = Assignment();
            return x;
        }

        private Expr Assignment()
        {
            var expr = Equality();

            if (Match(TokenType.IS))
            {
                Token isToken = Previous();
                //if (Match(TokenType.EQUAL))
                Expr value = Assignment();

                if (expr is Variable)
                {
                    Token name = ((Variable)expr).Name;
                    return new Assign(name, value);
                }
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
                    Token opr = Peek();
                    while (CheckNext(TokenType.TO) || Check(TokenType.TO)) Advance();
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
                
                while (MatchNext(TokenType.GREATER, TokenType.LESS))
                {
                    Token opr = Peek();
                    while (CheckNext(TokenType.THAN) || Check(TokenType.THAN)) Advance();
                    Expr right = Primary();
                    expr = new Binary(expr, opr, right);
                }
            }
            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.ADD, TokenType.SUBTRACT))
            {
                Token opr = Previous();
                Expr right = Factor();
                expr = new Binary(expr, opr, right);
            }

            return expr;

        }

        private Expr Factor()
        {
            Expr expr = Primary();
            while (Match(TokenType.DIVIDE, TokenType.MULTIPLY))
            {
                Token opr = Previous();
                while (CheckNext(TokenType.BY) || Check(TokenType.BY)) Advance();
                Expr right = Primary();
                expr = new Binary(expr, opr, right);

            }
            return expr;
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
            throw new Exception("error");
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

        private bool MatchNext(params TokenType[] types)
        {
            foreach (var tokentype in types)
            {
                if (CheckNext(tokentype))
                {
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
            throw new Exception(msg);
        }

        //public void Parse()
        //{
        //    while (!this._tokenizer.IsEof())
        //    {
                
        //    }
        //}

        //public bool IsPunc()
        //{
        //    var token = this._tokenizer.PeekToken();
        //    if (token.Type == "Punc")
        //    {
        //        return true;
        //    } 
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public bool IsKeyword()
        //{
        //    var token = this._tokenizer.PeekToken();
        //    if (token.Type == "Keyword")
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        
        //public bool isOperation()
        //{
        //    var token = this._tokenizer.PeekToken();
        //    if (token.Type == "Operation")
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        
    }
}
