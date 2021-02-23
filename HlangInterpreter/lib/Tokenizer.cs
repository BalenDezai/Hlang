using System;
using System.Collections.Generic;
using System.Text;
using HlangInterpreter.TokenEnums;

namespace HlangInterpreter.lib
{
    /// <summary>
    /// turns individual pieces of the code string into tokens
    /// </summary>
    public class Tokenizer
    {

        public List<Token> Tokens { get; set; } = new List<Token>();

        private readonly Scanner _scanner;
        private readonly Dictionary<string, TokenType> _keywords;

        public Tokenizer(Scanner scanner)
        {
            this._scanner = scanner;
            this._keywords = new Dictionary<string, TokenType>()
            {
                {"and", TokenType.AND },
                {"else", TokenType.ELSE },
                {"false", TokenType.FALSE},
                {"true", TokenType.TRUE},
                {"function", TokenType.FUNCTION },
                {"for", TokenType.FOR },
                {"each", TokenType.EACH},
                {"if", TokenType.IF },
                {"nothing", TokenType.NOTHING},
                {"while", TokenType.WHILE },
                {"print", TokenType.PRINT},
                {"return", TokenType.RETURN},
                {"equal", TokenType.EQUAL},
                {"is", TokenType.IS},
                {"not", TokenType.NOT},
                {"by", TokenType.BY },
                {"divide", TokenType.DIVIDE},
                {"multiply", TokenType.MULTIPLY},
                {"add", TokenType.ADD},
                {"subtract", TokenType.SUBTRACT},
                {"plus", TokenType.SUBTRACT},
                {"minus", TokenType.SUBTRACT},
                {"modulus", TokenType.MODULUS},
                {"greater", TokenType.GREATER},
                {"less", TokenType.LESS},
                {"than", TokenType.THAN },
                {"then", TokenType.THEN },
                {"or", TokenType.OR },
                {"define", TokenType.DEFINE },
                {"to", TokenType.TO }
            };

        }

        public List<Token> GetTokens()
        {
            while (!_scanner.IsEof())
            {
                _scanner.Start = _scanner.Position;
                ReadNextToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, _scanner.Line));
            return Tokens;
        }

        private void ReadNextToken()
        {
            //  TODO: handle if its whitespace start
            char c = _scanner.MoveToNextChar();
            switch (c)
            {
                case '\t': AddToken(TokenType.NEW_ENVIRONMENT); break;
                case ' ':
                    if (_scanner.Match(' '))
                    {
                        AddToken(TokenType.NEW_ENVIRONMENT);
                    }
                    break;
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '/': SkipComment(); break;
                case '\n': _scanner.Line++; break;
                case '"': ReadString(); break;
                default:
                    if (IsDigit(c))
                    {
                        ReadNumber();
                    } else if (IsAlphabet(c))
                    {
                       ReadWholeWord();
                    }
                    else
                    {
                        // throw error he re
                    }
                    break;
            }
        }

        /// <summary>
        /// Skips whole comment line
        /// </summary>
        private void SkipComment()
        {
            if (_scanner.Match('/'))
            {
                while (_scanner.PeekCurrentChar() != '\n' && !_scanner.IsEof()) _scanner.MoveToNextChar();
            }
            else if (_scanner.Match('*'))
            {
                while (true)
                {
                    if (_scanner.Match('*') && _scanner.Match('/'))
                    {
                        break;
                    }
                    _scanner.MoveToNextChar();
                }
            }
        }


        private void ReadNumber()
        {
            while (IsDigit(_scanner.PeekCurrentChar())) _scanner.MoveToNextChar();

            if (_scanner.PeekCurrentChar() == '.' && IsDigit(_scanner.PeekNextChar()))
            {
                _scanner.MoveToNextChar();
                while (IsDigit(_scanner.PeekCurrentChar())) _scanner.MoveToNextChar();
            }

            AddToken(TokenType.NUMBER, double.Parse(_scanner.GetStartToCurrent()));
        }


        private void ReadString()
        {
            while(_scanner.PeekCurrentChar() != '"' && !_scanner.IsEof())
            {
                if (_scanner.PeekCurrentChar() == '\n') _scanner.Line++;
                _scanner.MoveToNextChar();
            }
            if (_scanner.IsEof())
            {
                //throw error
                return;
            }

            _scanner.MoveToNextChar();
            string str = _scanner.CodeStr.Substring(_scanner.Start + 1, (_scanner.Position - _scanner.Start) - 2);
            AddToken(TokenType.STRING, str);
            
        }

        private void ReadWholeWord()
        {
            while (char.IsLetter(_scanner.PeekCurrentChar()) && !_scanner.IsEof()) _scanner.MoveToNextChar();
            var value = _scanner.GetStartToCurrent();
            TokenType type;
            if (!_keywords.TryGetValue(value, out type)) type = TokenType.IDENTIFER;
            AddToken(type);
        }

        private bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        private bool IsAlphabet(char c)
        {
            return char.IsLetter(c);
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }


        private void AddToken(TokenType  type, object literal)
        {
            string text = _scanner.GetStartToCurrent();
            Tokens.Add(new Token(type, text, literal, _scanner.Line));
        }
    }
}
