namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Helper class to scan over each character in a code sequence
    /// </summary>
    public class Scanner
    {
        public int Start { get; set; }
        public string CodeStr { get; set; }
        public int Position { get; set; }
        // Is begining of line
        public bool IsAbol { get; set; } = true;
        public int Line { get; set; }

        public Scanner(string codeStr)
        {
            CodeStr = codeStr;
            Position = 0;
        }

        /// <summary>
        /// Moves to the next character
        /// </summary>
        /// <returns>The previous character</returns>
        public char MoveToNextChar()
        {
            Position++;
            return CodeStr[Position- 1];
        }

        /// <summary>
        /// Peek current character in the sequence
        /// </summary>
        /// <returns>Current character</returns>
        public char PeekCurrentChar()
        {
            if (IsEof()) return '\0';
            return CodeStr[Position];
        }

        /// <summary>
        /// Peek next character in the sequence
        /// </summary>
        /// <returns>Next character</returns>
        public char PeekNextChar()
        {
            // if there is no next character return null character
            if (Position+ 1 >= CodeStr.Length) return '\0';
            return CodeStr[Position + 1];
        }

        /// <summary>
        /// Is end of the sequence
        /// </summary>
        public bool IsEof()
        {
            return Position >= CodeStr.Length;
        }

        /// <summary>
        ///  Check if current character matches the expected character.
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
        /// Get value from beginning position to current position
        /// </summary>
        /// <returns>String value</returns>
        public string GetStartToCurrent()
        {
            return CodeStr.Substring(Start, (Position - Start));
        }
    }
}
