using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat
{
    public class Amf3Trait
    {
        public string ClassName { get; set; }

        public bool IsAnonymous
        {
            get
            {
                return ClassName == "";
            }
        }

        public bool IsDynamic { get; set; }

        public bool IsExternalizable { get; set; }

        public List<string> Members { get; set; }

        public override string ToString()
        {
            if (IsDynamic)
            {
                return "Dynamic";
            }
            else if (IsExternalizable)
            {
                return "Extern " + ClassName;
            }
            else if (IsAnonymous)
            {
                return "Anonymous";
            }

            return "Class " + ClassName;
        }
    }
}