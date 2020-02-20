using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat
{
    public class Amf0Object
    {
        public string ClassName { get; set; }

        public bool IsAnonymous
        {
            get
            {
                return ClassName == "";
            }
        }

        public Dictionary<string, object> DynamicMembersAndValues { get; set; }

        public override string ToString()
        {
            if (IsAnonymous)
            {
                return "Anonymous";
            }

            return ClassName;
        }
    }
}