namespace HlangInterpreter.objects
{
    public class Conditional
    {
        public string Type { get; set; }
        public Token Cond { get; set; }
        public Token Then { get; set; }
        public Token Else { get; set; }
    }
}
