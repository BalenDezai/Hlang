using System.Collections.Generic;

namespace HlangInterpreter.objects
{
    public class Call
    {
        public string Type { get; set; }
        public object Func { get; set; }
        public List<Token> args { get; set; } = new List<Token>();
    }
}
