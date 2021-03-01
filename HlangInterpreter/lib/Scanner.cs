namespace HlangInterpreter.lib
{
    /// <summary>
    /// Helper class to characters and strings out of a code string
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
        /// Moves to the next character
        /// </summary>
        /// <returns>The previous characterr</returns>
        public char MoveToNextChar()
        {
            Position++;
            return CodeStr[Position- 1];
        }

        /// <summary>
        /// peek at what the current character in the sequence is is
        /// </summary>
        public char PeekCurrentChar()
        {
            if (IsEof()) return '\0';
            return CodeStr[Position];
        }

        /// <summary>
        /// Peek at what the next character in the sequence is
        /// </summary>
        /// <returns></returns>
        public char PeekNextChar()
        {
            // if there is no next character return null character
            if (Position+ 1 >= CodeStr.Length) return '\0';
            return CodeStr[Position + 1];
        }

        /// <summary>
        /// Is scanner at the end of the sequence
        /// </summary>
        public bool IsEof()
        {
            return Position >= CodeStr.Length;
        }

        /// <summary>
        ///  Check if current character in sequence matches the expected character
        ///  If it does match, character is then consumed
        /// </summary>
        public bool Match(char expected)
        {
            if (IsEof()) return false;
            if (CodeStr[Position] != expected) return false;
            Position++;
            return true;
        }
        /// <summary>
        /// Get the string value of begining position to current position in the sequence
        /// </summary>
        /// <returns></returns>
        public string GetStartToCurrent()
        {
            return CodeStr.Substring(Start, (Position - Start));
        }
    }
}
