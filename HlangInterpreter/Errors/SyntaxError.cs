using System;

namespace HlangInterpreter.Errors
{
    public class SyntaxError : Exception
    {
        public int Line { get; set; }
        public SyntaxError(int line, string message) : base (message)
        {
            Line = line;
        }
    }
}
