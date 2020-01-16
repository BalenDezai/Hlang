using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HlangInterpreter.lib
{
    /// <summary>
    /// turns individual pieces of the code string into tokens
    /// </summary>
    public class Tokenizer
    {
        private readonly Scanner _scanner;
        private readonly string _idStr;
        private readonly string _puncStr;
        private readonly string _whitespaceStr;
        private readonly string[] _keywords;
        private readonly Dictionary<string, string> _operationDictionary;
        private Token _currentToken;

        public Tokenizer(Scanner scanner)
        {
            this._scanner = scanner;
            this._idStr = "?!-<>=0123456789";
            this._puncStr = ",;(){}[]";
            this._whitespaceStr = "\n\t";
            this._keywords = new string[] { "fn", "if", "else",};
            this._operationDictionary = new Dictionary<string, string>()
            {
                {"equals", "=="},
                {"is", "="},
                {"not", "!"},
                {"divide", "/"},
                {"multiply", "*"},
                {"add", "+"},
                {"subtract", "-"},
                {"modulus", "%"},
                {"greater", ">"},
                {"less", "<"}

            };
            this._currentToken = null;
        }

        /// <summary>
        /// create a specific topic based on the current character
        /// </summary>
        /// <returns></returns>
        private Token ReadNextToken()
        {
            //  TODO: handle if its whitespace start
            
            if (this._scanner.IsEof()) return null;
            var ch = this._scanner.PeekCurrentChar();

            if (ch == '/')
            {
                this.SkipComment();
                return this.ReadNextToken();
            }

            if (ch == '"')
            {
                return this.ReadString(ch);
            }

            if (this.IsDigit(ch))
            {
                return this.ReadNumber(ch);
            }

            if (this.IsId(ch))
            {
                return this.ReadTxt(ch);
            }

            if (this.IsPunc(ch))
            {
                return new Token("Punc", this._scanner.MoveToNextChar());
            }

            if (char.IsWhiteSpace(ch))
            {
                this._scanner.MoveToNextChar();
                return ReadNextToken();
            }

            throw new Exception("Error in read next char");
        }

        /// <summary>
        /// Skips whole comment line
        /// </summary>
        private void SkipComment()
        {
            this.ReadWhile((ch) => ch != '\n');
            this._scanner.MoveToNextChar();
        }

        /// <summary>
        /// reads a string of digits,
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>token with the digits parsed</returns>
        private Token ReadNumber(char ch)
        {
            var isFloat = false;
            var numberStr = ReadWhile((chToCheck) =>
            {
                if (chToCheck == '.')
                {
                    if (isFloat) return false;
                    isFloat = true;
                    return true;
                }
                return this.IsDigit(chToCheck);
            });

            if (isFloat)
            {
                return new Token("Float", float.Parse(numberStr));
            } else
            {
                return new Token("Integer", int.Parse(numberStr));
            }
        }

        /// <summary>
        /// reads string in quotation marks
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>token with a string value</returns>
        private Token ReadString(char ch)
        {
            return new Token("String", this.ReadRestOfString());
        }

        /// <summary>
        /// reads characters based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>value read as a string</returns>
        private string ReadWhile(Func<char, bool> predicate)
        {
            var strBuilder = new StringBuilder();

            while (!this._scanner.IsEof() && predicate(this._scanner.PeekCurrentChar()))
            {
                strBuilder.Append(this._scanner.MoveToNextChar());
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// reads a sequence of string.
        /// will read even if escaped
        /// </summary>
        /// <returns>string sequence</returns>
        private string ReadRestOfString()
        {
            bool escaped = false;
            var strBuilder = new StringBuilder();
            this._scanner.MoveToNextChar();

            while (!this._scanner.IsEof())
            {
                var ch = this._scanner.MoveToNextChar();
                if (escaped)
                {
                    strBuilder.Append(ch);
                    escaped = false;
                }
                else if (ch == '\\')
                {
                    escaped = true;
                }
                else if (ch == '"')
                {
                    break;
                }
                else
                {
                    strBuilder.Append(ch);
                }
            }

            return strBuilder.ToString();
        }

        private bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        private bool IsIdStart(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }

        private bool IsId(char ch)
        {
            if (IsIdStart(ch)) return true;
            for (int i = 0; i < _idStr.Length; i++)
            {
                if (_idStr[i] == ch) return true;
            }
            return false;
        }

        private bool Iswhitespace(char ch)
        {
            for (int i = 0; i < _whitespaceStr.Length; i++)
            {
                if (_whitespaceStr[i] == ch) return true;
            }
            return false;
        }

        private bool IsPunc(char ch)
        {
            for (int i = 0; i < _puncStr.Length; i++)
            {
                if (_puncStr[i] == ch) return true;
            }
            return false;
        }

        public Token ReadTxt(char c)
        {
            var str = ReadWhile(this.IsId);

            var isKeyword = this.IsKeyword(str);
            if (isKeyword)
            {
                return new Token("Keyword", str);
            }

            var isOperation = this.IsOperation(str);
            if (isOperation)
            {
                return new Token("Operation", this._operationDictionary[str], str);
            }
            return new Token("Variable", str);
        }

        private bool IsKeyword(string keyword)
        {
            for (int i = 0; i < this._keywords.Length; i++)
            {
                if (this._keywords[i] == keyword) return true;
            }
            return false;
        }

        private bool IsOperation(string operation)
        {
            return this._operationDictionary.ContainsKey(operation);
        }

        public Token PeekToken()
        {
            if (this._currentToken == null)
            {
                this._currentToken = ReadNextToken();
            }
            return this._currentToken;
        }

        public Token NextToken()
        {
            var token = this._currentToken;
            this._currentToken = null;
            if (token == null)
            {
                return this.ReadNextToken();
            }
            return token;
        }

        public bool IsEof()
        {
            return PeekToken() == null;
        }
    }
}
