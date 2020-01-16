using System;
using System.Collections.Generic;
using System.Text;
using HlangInterpreter.lib;

namespace HlangInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = "sum is fn(x,y) { x add y; } print(sum(5, 6));";

            var str2 = "sum is fn(x, y) { x add y; } sumOfAddition is sum(5, 6); if sumOfAddition is greater than 12 print(sumOfAddition);";
            var tokenizer = new Tokenizer(new Scanner(str));
            while (!tokenizer.IsEof())
            {
                Console.WriteLine(tokenizer.PeekToken());
                tokenizer.NextToken();
            }
        }
    }
}
