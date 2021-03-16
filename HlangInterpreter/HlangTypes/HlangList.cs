using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes
{
    /// <summary>
    /// Represents a list in Hlang
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HlangList<T> : List<T>
    {
        public override string ToString()
        {

            return $"[{string.Join(", ", this)}]";
        }
    }
}
