using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        public void Read(object value)
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
                var anonymous = value.GetType().GetCustomAttributes<CompilerGeneratedAttribute>().FirstOrDefault();

                if (anonymous != null)
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
                    else
                    {
                        ClassName = value.GetType().Name;
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

        public object Object()
        {
            throw new NotImplementedException();
        }
    }
}