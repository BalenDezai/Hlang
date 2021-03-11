using HlangInterpreter.Errors;
using HlangInterpreter.Statements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HlangInterpreter.Lib
{
    public class HumanLanguage
    {
        private readonly Interpreter _interpreter;
        private readonly ErrorReporting _errorReporting;

        public HumanLanguage()
        {
            _interpreter = new Interpreter();
            _errorReporting = new ErrorReporting();
        }
        public void RunFromFile(string filePath)
        {
            //byte[] bytes = File.ReadAllText(filePath);
            //string code = Encoding.Default.GetString(bytes);
            string code = File.ReadAllText(filePath);
            RunCode(code);
        }
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


        private void RunCode(string code)
        {
            var tokenizer = new Tokenizer(new Scanner(code));
            Parser parser = new Parser(_errorReporting);
            try
            {
                List<Statement> statements = parser.Parse(tokenizer.GetTokens());
                _interpreter.Interpret(statements);
            }
            catch (SyntaxError err)
            {
                ReportError(err.Line, "", err.Message);
            }
        }

        private void ReportError(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error at '{where}': {message}");
        }

    }
}
