using System.Collections.Generic;
using System.Globalization;
using HlangInterpreter.Errors;
using HlangInterpreter.TokenEnums;

namespace HlangInterpreter.lib
{
    /// <summary>
    /// Creates a list of tokens out of a sequence of code
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
                {"plus", TokenType.ADD},
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
                case '\t': AddToken(TokenType.INDENT); break;
                case ' ': break;
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '/': SkipComment(); break;
                case '\n':
                    _scanner.Line++;
                    if (_scanner.Match(' '))
                    {
                        AddToken(TokenType.INDENT);
                    }
                    break;
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
                        throw new SyntaxError(_scanner.Line, "Can't process character");
                    }
                    break;
            }
        }

        /// <summary>
        /// Skip both  //  and /* */ comments
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

        /// <summary>
        /// Reads the whole number in the sequence of characters and adds the number token
        /// </summary>
        private void ReadNumber()
        {
            // while the character is a digit, move forward
            while (IsDigit(_scanner.PeekCurrentChar())) _scanner.MoveToNextChar();

            // if we hit a period then continue
            if (_scanner.PeekCurrentChar() == '.' && IsDigit(_scanner.PeekNextChar()))
            {
                _scanner.MoveToNextChar();
                while (IsDigit(_scanner.PeekCurrentChar())) _scanner.MoveToNextChar();
            }

            // parse the literal and add to a number token
            var test = _scanner.GetStartToCurrent();
            AddToken(TokenType.NUMBER, double.Parse(test, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Reads a string literal in the sequence of characters and adds the string token
        /// </summary>
        private void ReadString()
        {
            // move forward while we are not at the end of the string or end of file/sequence
            while(_scanner.PeekCurrentChar() != '"' && !_scanner.IsEof())
            {
                if (_scanner.PeekCurrentChar() == '\n') _scanner.Line++;
                _scanner.MoveToNextChar();
            }

            if (_scanner.IsEof())
            {
                throw new SyntaxError(_scanner.Line, "Unterminated string");
            }

            _scanner.MoveToNextChar();
            // extract the string without the quotation marks
            string str = _scanner.CodeStr.Substring(_scanner.Start + 1, (_scanner.Position - _scanner.Start) - 2);
            AddToken(TokenType.STRING, str);
            
        }
        /// <summary>
        /// Reads a whole word (not string literal) in the sequence and adds the proper token
        /// </summary>
        private void ReadWholeWord()
        {
            while (char.IsLetter(_scanner.PeekCurrentChar()) && !_scanner.IsEof()) _scanner.MoveToNextChar();
            var value = _scanner.GetStartToCurrent();
            TokenType type;
            // if the value does not exist keywords, it must be an identifer
            if (!_keywords.TryGetValue(value, out type)) type = TokenType.IDENTIFER;
            AddToken(type);
        }
        /// <summary>
        /// Check if a character is a digit
        /// </summary>
        /// <param name="ch">Character to check</param>
        private bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        /// <summary>
        /// Check if a character is an alphabet
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns></returns>
        private bool IsAlphabet(char c)
        {
            return char.IsLetter(c);
        }

        /// <summary>
        /// Add token to the token list
        /// </summary>
        /// <param name="type">Type of token to add</param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Add token to the token list
        /// </summary>
        /// <param name="type">Type of token to add</param>
        /// <param name="literal">Literal of the token</param>
        private void AddToken(TokenType  type, object literal)
        {
            string text = _scanner.GetStartToCurrent();
            Tokens.Add(new Token(type, text, literal, _scanner.Line));
        }
    }
}
