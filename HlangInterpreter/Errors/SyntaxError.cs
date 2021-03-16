using System;

namespace HlangInterpreter.Errors
{
    /// <summary>
    /// Code syntax error exception
    /// </summary>
    public class SyntaxError : Exception
    {
        /// <summary>
        /// Line where error happened
        /// </summary>
        public int Line { get; set; }
        public SyntaxError(int line, string message) : base (message)
        {
            Line = line;
        }
    }
}
