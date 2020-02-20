using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

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

        public void FromObject(object value)
        {
            if (value is ExpandoObject)
            {
                foreach (var item in ( IDictionary<string, object> )value)
                {
                    DynamicMembersAndValues.Add(item.Key, item.Value);
                }
            }
            else
            {
                var traitClass = value.GetType().GetCustomAttributes<TraitClassAttribute>().FirstOrDefault();

                if (traitClass != null)
                {
                    ClassName = traitClass.Name;
                }

                foreach (var property in value.GetType().GetProperties() )
                {
                    var traitMember = property.GetCustomAttribute<TraitMemberAttribute>();

                    if (traitMember != null)
                    {
                        DynamicMembersAndValues.Add(traitMember.Name, property.GetValue(value) );
                    }
                    else
                    {
                        DynamicMembersAndValues.Add(property.Name, property.GetValue(value) );
                    }
                }
            }
        }
    }
}