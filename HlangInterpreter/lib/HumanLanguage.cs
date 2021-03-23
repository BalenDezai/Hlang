using HlangInterpreter.Errors;
using HlangInterpreter.Statements;
using System;
using System.Collections.Generic;
using System.IO;

namespace HlangInterpreter.Lib
{
    /// <summary>
    /// Hlang main class that pieces it all together
    /// </summary>
    public class HumanLanguage
    {
        private readonly Interpreter _interpreter;
        private readonly ErrorReporting _errorReporting;

        public HumanLanguage()
        {
            _interpreter = new Interpreter();
            _errorReporting = new ErrorReporting();
        }
        /// <summary>
        /// Execute code from a file
        /// </summary>
        /// <param name="filePath">File path</param>
        public void RunFromFile(string filePath)
        {
            string code = File.ReadAllText(filePath);
            RunCode(code);
        }
        /// <summary>
        /// Start up the Repl to run lines of code
        /// </summary>
        public void StartRepl()
        {
            while (true)
            {
                Console.Write(">> ");
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                RunCode(line);
            }
        }

        /// <summary>
        /// Execute the code given from a file or REPL
        /// </summary>
        /// <param name="code">Code string</param>
        private void RunCode(string code)
        {
            var tokenizer = new Tokenizer(new Scanner(code));
            Parser parser = new Parser(_errorReporting);
            try
            {
                List<Statement> statements = parser.Parse(tokenizer.GetTokens());
                SemanticAnalyzer sa = new SemanticAnalyzer(_interpreter);
                sa.Analyze(statements);
                _interpreter.Interpret(statements);
            }
            catch (SyntaxError err)
            {
                _errorReporting.ReportError(err.Line,  err.Message);
            }
            catch (SemanticError err)
            {
                _errorReporting.ReportError(err.Line, err.Token.Lexeme, err.Message);
            }
            catch (RuntimeError err)
            {
                _errorReporting.ReportError(err.Token.Lexeme, err.Message);
            }
        }

    }
}
