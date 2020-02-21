using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace mtanksl.ActionMessageFormat
{
    public class Amf3Object
    {
        public Amf3Trait Trait { get; set; }

        public List<object> Values { get; set; }

        public Dictionary<string, object> DynamicMembersAndValues { get; set; }

        public override string ToString()
        {
            return Trait.ToString();
        }

        public void Read(object value)
        {
            if (value is ExpandoObject)
            {
                Trait.IsDynamic = true;

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
                    Trait.IsDynamic = true;

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
                        Trait.ClassName = traitClass.Name;
                    }
                    else
                    {
                        Trait.ClassName = value.GetType().Name;
                    }

                    if (value is IExternalizable)
                    {
                        Trait.IsExternalizable = true;
                    }
                    else
                    {
                        foreach (var property in value.GetType().GetProperties() )
                        {
                            var traitMember = property.GetCustomAttribute<TraitMemberAttribute>();

                            if (traitMember != null)
                            {
                                Trait.Members.Add(traitMember.Name);

                                Values.Add(property.GetValue(value) );
                            }
                            else
                            {
                                Trait.Members.Add(property.Name);

                                Values.Add(property.GetValue(value) );
                            }
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