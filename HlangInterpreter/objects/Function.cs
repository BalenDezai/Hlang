using System.Collections.Generic;

namespace HlangInterpreter.objects
{
    public class Function
    {
        public string Type { get; set; }
        public List<string> Variables { get; set; } = new List<string>();
        public Token Body { get; set; }

        public void AddVar(string variable)
        {
            this.Variables.Add(variable);
        }
    } 
}
