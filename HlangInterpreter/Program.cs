using HlangInterpreter.Lib;
using System;

namespace HlangInterpreter
{
    class Program
    {

        static void Main(string[] args)
        {
            HumanLanguage hLang = new HumanLanguage();

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: Hlang {fileName}.Hl");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                hLang.RunFromFile(args[0]);
            }
            else
            {
                hLang.StartRepl();
            }
        }

    }
}
