namespace HlangInterpreter.lib
{
    /// <summary>
    /// goes over the string of code character by character
    /// </summary>
    public class Scanner
    {
        public int Start { get; set; }
        public string CodeStr { get; set; }
        public int Position { get; set; }
        public int Line { get; set; }

        public Scanner(string codeStr)
        {
            CodeStr = codeStr;
            Position = 0;
        }

        /// <summary>
        /// returns current character and moves to the next character
        /// </summary>
        /// <returns>current character</returns>
        public char MoveToNextChar()
        {
            Position++;
            return CodeStr[Position- 1];
        }

        /// <summary>
        /// peek at what the current character is
        /// </summary>
        /// <returns>current character</returns>
        public char PeekCurrentChar()
        {
            if (IsEof()) return '\0';
            return CodeStr[Position];
        }

        public char PeekNextChar()
        {
            if (Position+ 1 >= CodeStr.Length) return '\0';
            return CodeStr[Position + 1];
        }

        /// <summary>
        /// checks to see if at the end of string
        /// </summary>
        /// <returns>boolean value indicating if at end of line or not</returns>
        public bool IsEof()
        {
            return Position >= CodeStr.Length;
        }

        public bool Match(char expected)
        {
            if (IsEof()) return false;
            if (CodeStr[Position] != expected) return false;
            Position++;
            return true;
        }

        public string GetStartToCurrent()
        {
            return CodeStr.Substring(Start, (Position - Start));
        }
    }
}
