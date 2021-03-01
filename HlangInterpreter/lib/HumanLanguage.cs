using HlangInterpreter.Errors;
using HlangInterpreter.StmtLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HlangInterpreter.lib
{
    public class HumanLanguage
    {
        private Interpreter _interpreter;
        public HumanLanguage()
        {
            _interpreter = new Interpreter();
        }
        public void RunFromFile(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            string code = Encoding.Default.GetString(bytes);
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
            Parser parser = new Parser();
            try
            {
                List<Statement> statements = parser.Parse(tokenizer.GetTokens());
                _interpreter.Interpret(statements);
            }
            catch (SyntaxError err)
            {
                ReportError(err.Line, "", err.Message);
            }
            catch (ParsingError err)
            {
                ReportError(err.Token.Line, $"At {err.Token.Lexeme}", err.Message);
                parser.SynchronizeParsing();
            }
        }

        private void ReportError(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error{where}: {message}");
        }

    }
}
