using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace mtanksl.ActionMessageFormat
{
    public class Amf0Object : IAmfObject
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

            return "Class " + ClassName;
        }

        public void FromObject(object value)
        {
            var traitClass = value.GetType().GetCustomAttributes<TraitClassAttribute>().FirstOrDefault();

            if (traitClass != null)
            {
                ClassName = traitClass.Name;
                    
                foreach (var property in value.GetType().GetProperties() )
                {
                    var traitMember = property.GetCustomAttribute<TraitMemberAttribute>();

                    if (traitMember != null && traitMember.Serializable)
                    {
                        DynamicMembersAndValues.Add(traitMember.Name, property.GetValue(value) );
                    }
                }
            }
            else
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
                    foreach (var property in value.GetType().GetProperties() )
                    {
                        DynamicMembersAndValues.Add(property.Name, property.GetValue(value) );
                    }
                }
            }
        }
        
        private object toObject;

        public T ToObject<T>()
        {
            return (T)ToObject();
        }

        public T ToObject<T>(IAmfSerializer serializer)
        {
            return (T)ToObject(serializer);
        }

        public object ToObject()
        {
            return ToObject(AmfSerializer.Default);
        }

        public object ToObject(IAmfSerializer serializer)
        {
            if (toObject == null)
            {
                try
                {
                    if (IsAnonymous)
                    {
                        var instance = new ExpandoObject();

                        toObject = instance;

                        foreach (var item in DynamicMembersAndValues)
                        {
                            ( ( IDictionary<string, object> )instance ).Add(item.Key, serializer.Normalize(item.Value) );
                        }
                    }
                    else
                    {
                        var type = AmfSerializer.GetTypeByTraitClassName(ClassName);

                        if (type == null)
                        {
                            throw new Exception("Can not find class " + ClassName);
                        }
                    
                        var instance = Activator.CreateInstance(type);

                        toObject = instance;

                        foreach (var pair in DynamicMembersAndValues)
                        {
                            var property = type.GetProperties().Where(p => p.GetCustomAttributes<TraitMemberAttribute>().Where(a => a.Name == pair.Key).Any() ).FirstOrDefault();

                            if (property == null)
                            {
                                if (serializer.ThrowIfPropertyNotFound)
                                {
                                    throw new Exception("Can not find property " + pair.Key + " in class " + ClassName);
                                }
                            }
                            else
                            { 
                                var value = serializer.Normalize(pair.Value);

                                try
                                {
                                    property.SetValue(instance, value);
                                }
                                catch
                                {
                                    property.SetValue(instance, Convert.ChangeType(value, property.PropertyType) );
                                }
                            }
                        }
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
    }
}