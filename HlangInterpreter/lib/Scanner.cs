using System;
using System.Collections.Generic;
using System.Text;

namespace HlangInterpreter.lib
{
    public class Scanner
    {
        private readonly string codeStr;
        private int position;

        public Scanner(string codeStr)
        {
            this.codeStr = codeStr;
            this.position = 0;
        }

        public char NextChar()
        {
            
            return this.codeStr[this.position++];
            
        }

        public char PeekCurrentChar()
        {
            try
            {
                var ch = this.codeStr[this.position];
                return ch;
            }
            catch (IndexOutOfRangeException)
            {
                return '\0';
            }
        }

        public bool IsEof()
        {
            var ch = this.PeekCurrentChar();
            if (ch.Equals(char.MinValue)) return true;
            return false;
        }


    }
}
