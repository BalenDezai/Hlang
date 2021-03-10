using System;

namespace HlangInterpreter.Errors
{
    public class ErrorReporting
    {
        public void ReportError(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error at '{where}': {message}");
        }
    }
}
