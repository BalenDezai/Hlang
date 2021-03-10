using System.Collections.Generic;

namespace HlangInterpreter.HlangTypes
{
    public class HlangList<T> : List<T>
    {

        public override string ToString()
        {

            return $"[{string.Join(", ", this)}]";
        }
    }
}
