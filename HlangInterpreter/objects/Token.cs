namespace HlangInterpreter.objects
{
    public class Token
    {
        public string Type { get; set; }
        public dynamic Value { get; set; }
        public string PsuedoValue { get; set; }
        public Token(string type, dynamic value)
        {
            this.Type = type;
            this.Value = value;
        }
        public Token(string type, dynamic value, string pseudoValue)
        {
            this.Type = type;
            this.Value = value;
            this.PsuedoValue = pseudoValue;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(PsuedoValue))
            {
                return $"type: {this.Type}, value: {this.Value}";
            }
            return $"type: {this.Type}, value: {this.Value}, pseudoValue: {this.PsuedoValue}";
        }
    }
}
