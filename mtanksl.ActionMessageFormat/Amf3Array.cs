using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat
{
    public class Amf3Array
    {
        public List<object> StrictDense { get; set; }

        public Dictionary<string, object> SparseAssociative { get; set; }

        public override string ToString()
        {
            return "Array with " + StrictDense.Count.ToString() + " and " + SparseAssociative.Count + " items";
        }
    }
}