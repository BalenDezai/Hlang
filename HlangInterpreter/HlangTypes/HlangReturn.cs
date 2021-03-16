using System;

namespace HlangInterpreter.HlangTypes
{
    /// <summary>
    /// Represents a return statement in Hlang
    /// </summary>
    public class HlangReturn : Exception
    {
        public object Value { get; set; }
        public HlangReturn(object value)
        {
            Value = value;
        }
    }
}
