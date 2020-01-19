namespace HlangInterpreter.objects
{
    public class Assign
    {
        public string Type { get; set; }
        public string Operator { get; set; }
        public Token Left { get; set; }
        public Token Right { get; set; }
    }
}
