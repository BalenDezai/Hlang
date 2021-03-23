using System;

namespace HlangInterpreter.Errors
{
    /// <summary>
    /// Helper class to report errors
    /// </summary>
    public class ErrorReporting
    {
        /// <summary>
        /// Print an error message
        /// </summary>
        /// <param name="line">Line of the error</param>
        /// <param name="where">Where the error happened</param>
        /// <param name="message">Error message</param>
        public void ReportError(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error at '{where}': {message}");
        }

        public void ReportError(string where, string message)
        {
            Console.WriteLine($"Error at '{where}': {message}");
        }

        public void ReportError(int line, string message)
        {
            Console.WriteLine($"[line {line}] : {message}");
        }
    }
}
