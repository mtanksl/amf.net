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

                    if (traitMember != null)
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

        public object ToObject()
        {
            if (toObject == null)
            {
                object instance;

                if (IsAnonymous)
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
                else
                {
                    var type = Util.GetTypeByTraitClassName(ClassName);

                    if (type == null)
                    {
                        throw new Exception("Can not find class " + ClassName);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type);

                        foreach (var pair in DynamicMembersAndValues)
                        {
                            var property = type.GetProperties().Where(p => p.GetCustomAttributes<TraitMemberAttribute>().Where(a => a.Name == pair.Key).Any() ).FirstOrDefault();

                            if (property != null)
                            {
                                var value = pair.Value;

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

                toObject = instance;
            }

            return toObject;
        }
    }
}