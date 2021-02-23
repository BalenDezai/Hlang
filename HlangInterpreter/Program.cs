using System;
using System.Collections.Generic;
using System.Text;
using HlangInterpreter.ExprLib;
using HlangInterpreter.lib;

namespace HlangInterpreter
{
    class Program
    {
        private static bool _hadSyntaxError = false;
        private static bool _hadRuntimeERror = false;
        static void Main(string[] args)
        {

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: Hlang {fileName}.Hlang");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFromFile(args[0]);
            }
            else
            {
                StartRepl();
            }
        }

        private static void StartRepl()
        {
            for (; ; )
            {
                Console.Write(">> ");
                string line = Console.ReadLine();
                var tokens = new Tokenizer(new Scanner(line)).GetTokens();
                Parser parser = new Parser(tokens);
                List<Expr> expressions = parser.Parse();
                foreach (var expression in expressions)
                {
                    Console.WriteLine(expression.ToString());
                }
                if (_hadSyntaxError) return;
                RunCode(line);
            }

        }

        private static void RunFromFile(string filePath)
        {
            
        }

        private static void RunCode(string code)
        {

        }

    }
}
