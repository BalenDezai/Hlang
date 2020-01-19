namespace HlangInterpreter.objects
{
    public class Binary
    {
        public string Type { get; set; }
        public string Operator { get; set; }
        public object Left { get; set; }
        public Binary Right { get; set; }
    }
}
