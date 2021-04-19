namespace HlangInterpreter.HlangTypes.HlangClassHelpers
{
    public class ClassField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPrivate { get; set; }
        public ClassField(string name, object value, bool isStatic = false, bool isPrivate = false)
        {
            Name = name;
            Value = value;
            IsStatic = isStatic;
            IsPrivate = isPrivate;
        }
    }
}
