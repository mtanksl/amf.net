using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat
{
    public class Amf3Array : IAmfObject
    {
        public List<object> StrictDense { get; set; }

        public Dictionary<string, object> SparseAssociative { get; set; }

        private object toObject;

        public object ToObject()
        {
            return ToObject(AmfSerializer.Default);
        }

        public object ToObject(AmfSerializer serializer)
        {
            if (toObject == null)
            {
                try
                {
                    var instance = new Amf3Array()
                    {
                        StrictDense = new List<object>(),

                        SparseAssociative = new Dictionary<string, object>()
                    };

                    toObject = instance;

                    foreach (var item in StrictDense)
                    {
                        instance.StrictDense.Add(serializer.Normalize(item) );
                    }

                    foreach (var item in SparseAssociative)
                    {
                        instance.SparseAssociative.Add(item.Key, serializer.Normalize(item.Value) );
                    }
                }
                catch
                {
                    toObject = null;

                    throw;
                }
            }

            return toObject;
        }

        public override string ToString()
        {
            return "Array with " + StrictDense.Count.ToString() + " items and " + SparseAssociative.Count + " key-value pairs";
        }
    }
}