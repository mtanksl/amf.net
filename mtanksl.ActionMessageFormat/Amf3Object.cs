using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace mtanksl.ActionMessageFormat
{
    public class Amf3Object
    {
        public Amf3Trait Trait { get; set; }

        public List<object> Values { get; set; }

        public Dictionary<string, object> DynamicMembersAndValues { get; set; }
        
        public void FromObject(object value)
        {
            Trait = new Amf3Trait()
            {
                ClassName = "",

                IsDynamic = false,

                IsExternalizable = false,

                Members = new List<string>()
            };

            Values = new List<object>();

            DynamicMembersAndValues = new Dictionary<string, object>();

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
                var traitClass = value.GetType().GetCustomAttributes<TraitClassAttribute>().FirstOrDefault();

                if (traitClass == null)
                {
                    foreach (var property in value.GetType().GetProperties() )
                    {
                        Trait.Members.Add(property.Name);

                        Values.Add(property.GetValue(value) );
                    }
                }
                else
                {
                    Trait.ClassName = traitClass.Name;

                    if (value is IExternalizable)
                    {
                        Trait.IsExternalizable = true;
                    }

                    foreach (var property in value.GetType().GetProperties() )
                    {
                        var traitMember = property.GetCustomAttribute<TraitMemberAttribute>();

                        if (traitMember != null)
                        {
                            Trait.Members.Add(traitMember.Name);

                            Values.Add(property.GetValue(value) );
                        }
                    }
                }
            }
        }

        public object ToObject()
        {
            if (Trait.IsDynamic)
            {
                var value = new ExpandoObject();

                foreach (var item in DynamicMembersAndValues)
                {
                    ( ( IDictionary<string, object> )value ).Add(item.Key, item.Value);
                }

                return value;
            }

            if (Trait.IsAnonymous)
            {
                var value = new ExpandoObject();

                for (int i = 0; i < Trait.Members.Count; i++)
                {
                    ( ( IDictionary<string, object> )value ).Add(Trait.Members[i], Values[i] );
                }

                return value;
            }

            var type = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes<TraitClassAttribute>().Where(a => a.Name == Trait.ClassName).Any() ).FirstOrDefault();

            if (type == null)
            {
                throw new Exception("Can not instanciate class with trait name " + Trait.ClassName);
            }
            else
            {
                var value = Activator.CreateInstance(type);

                if (Trait.Members.Count == Values.Count)
                {
                    for (int i = 0; i < Trait.Members.Count; i++)
                    {
                        var property = type.GetProperties().Where(p => p.GetCustomAttribute<TraitMemberAttribute>()?.Name == Trait.Members[i] ).FirstOrDefault();

                        if (property != null)
                        {
                            var obj = Values[i];

                            if (obj is Amf3Object)
                            {
                                obj = ( (Amf3Object)obj ).ToObject();
                            }

                            property.SetValue(value, obj);
                        }
                    }
                }
                
                return value;
            }
        }

        public override string ToString()
        {
            if (Trait.IsDynamic)
            {
                return "Dynamic";
            }
            else if (Trait.IsExternalizable)
            {
                return "Extern " + Trait.ClassName;
            }
            else if (Trait.IsAnonymous)
            {
                return "Anonymous";
            }

            return Trait.ClassName;
        }
    }
}