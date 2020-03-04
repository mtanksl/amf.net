using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace mtanksl.ActionMessageFormat
{
    public class Amf3Object : IAmfObject
    {
        public Amf3Trait Trait { get; set; }

        public List<object> Values { get; set; }

        public Dictionary<string, object> DynamicMembersAndValues { get; set; }

        public override string ToString()
        {
            return Trait.ToString();
        }

        public void FromObject(object value)
        {
            var traitClass = value.GetType().GetCustomAttributes<TraitClassAttribute>().FirstOrDefault();

            if (traitClass != null)
            {               
                if (value is IExternalizable)
                {
                    Trait.IsExternalizable = true;
                }

                Trait.ClassName = traitClass.Name;
                
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
            else
            {
                Trait.IsDynamic = true;

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

        public T ToObject<T>(AmfSerializer serializer)
        {
            return (T)ToObject(serializer);
        }

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
                    if (Trait.IsDynamic)
                    {
                        var instance = new ExpandoObject();

                        toObject = instance;

                        foreach (var item in DynamicMembersAndValues)
                        {
                            ( (IDictionary<string, object>)instance ).Add(item.Key, serializer.Normalize(item.Value) );
                        }
                    }
                    else if (Trait.IsAnonymous)
                    {
                        var instance = new ExpandoObject();

                        toObject = instance;

                        for (int i = 0; i < Trait.Members.Count; i++)
                        {
                            ( (IDictionary<string, object>)instance ).Add(Trait.Members[i], serializer.Normalize(Values[i] ) );
                        }
                    }
                    else
                    {
                        var type = AmfSerializer.GetTypeByTraitClassName(Trait.ClassName);

                        if (type == null)
                        {
                            throw new Exception("Can not find class " + Trait.ClassName);
                        }
                    
                        var instance = Activator.CreateInstance(type);

                        toObject = instance;

                        for (int i = 0; i < Trait.Members.Count; i++)
                        {
                            var property = type.GetProperties().Where(p => p.GetCustomAttributes<TraitMemberAttribute>().Where(a => a.Name == Trait.Members[i] ).Any() ).FirstOrDefault();

                            if (property == null)
                            {
                                if (serializer.ThrowIfPropertyNotFound)
                                {
                                    throw new Exception("Can not find property " + Trait.Members[i] + " in class " + Trait.ClassName);
                                }
                            }
                            else
                            { 
                                var value = serializer.Normalize(Values[i] );

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