using System.Collections.Generic;

namespace HlangInterpreter.objects
{
    public class Prog
    {
        public string Type { get; set; }
        public List<dynamic> Program { get; set; } = new List<dynamic>();
    }
}
