using System;
using HlangInterpreter.lib;

namespace HlangInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = "erwerdfdsf 1444";
            var s = new Scanner(str);
            while (!s.IsEof())
            {
                Console.WriteLine(s.PeekCurrentChar());
                s.NextChar();
            }
        }
    }
}
