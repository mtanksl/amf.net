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

        public object ToObject()
        {
            if (toObject == null)
            {
                object instance;

                if (Trait.IsDynamic)
                {
                    instance = new ExpandoObject();

                    foreach (var item in DynamicMembersAndValues)
                    {
                        var value = item.Value;

                        if (value is IAmfObject)
                        {
                            value = ( (IAmfObject)value ).ToObject();
                        }

                        ( ( IDictionary<string, object> )instance ).Add(item.Key, value);
                    }
                }
                else if (Trait.IsAnonymous)
                {
                    instance = new ExpandoObject();

                    if (Trait.Members.Count == Values.Count)
                    {
                        for (int i = 0; i < Trait.Members.Count; i++)
                        {
                            var value = Values[i];

                            if (value is IAmfObject)
                            {
                                value = ( (IAmfObject)value ).ToObject();
                            }

                            ( (IDictionary<string, object> )instance ).Add(Trait.Members[i], value);
                        }
                    }
                }
                else
                {
                    var type = Util.GetTypeByTraitClassName(Trait.ClassName);

                    if (type == null)
                    {
                        throw new Exception("Can not instanciate class " + Trait.ClassName);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type);

                        if (Trait.Members.Count == Values.Count)
                        {
                            for (int i = 0; i < Trait.Members.Count; i++)
                            {
                                var property = type.GetProperties().Where(p => p.GetCustomAttributes<TraitMemberAttribute>().Where(a => a.Name == Trait.Members[i] ).Any() ).FirstOrDefault();

                                if (property != null)
                                {
                                    var value = Values[i];

                                    if (value is IAmfObject)
                                    {
                                        value = ( (IAmfObject)value ).ToObject();
                                    }

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
                }

                toObject = instance;
            }

            return toObject; 
        }   
    }
}