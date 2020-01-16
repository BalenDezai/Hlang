namespace HlangInterpreter.lib
{
    /// <summary>
    /// goes over the string of code character by character
    /// </summary>
    public class Scanner
    {
        private readonly string _codeStr;
        private int _position;

        public Scanner(string codeStr)
        {
            this._codeStr = codeStr + char.MinValue;
            this._position = 0;
        }

        /// <summary>
        /// returns current character and moves to the next character
        /// </summary>
        /// <returns>current character</returns>
        public char MoveToNextChar()
        {
            return this._codeStr[this._position++];
        }

        /// <summary>
        /// peek at what the current character is
        /// </summary>
        /// <returns>current character</returns>
        public char PeekCurrentChar()
        {
            var ch = this._codeStr[this._position];
            return ch;
        }

        /// <summary>
        /// checks to see if at the end of string
        /// </summary>
        /// <returns>boolean value indicating if at end of line or not</returns>
        public bool IsEof()
        {
            var ch = this.PeekCurrentChar();
            if (ch.Equals(char.MinValue)) return true;
            return false;
        }


    }
}
