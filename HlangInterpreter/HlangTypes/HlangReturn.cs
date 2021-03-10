using System;

namespace HlangInterpreter.HlangTypes
{
    public class HlangReturn : Exception
    {
        public object Value { get; set; }
        public HlangReturn(object value)
        {
            Value = value;
        }
    }
}
