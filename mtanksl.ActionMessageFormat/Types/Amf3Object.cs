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

        public override string ToString()
        {
            return Trait.ToString();
        }

        public void Read(object value)
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

        public object ToObject
        {
            get
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
                                ( ( IDictionary<string, object> )instance ).Add(item.Key, item.Value);
                            }
                        }
                        else
                        {
                            Type type = null;

                            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies() )
                            {
                                bool found = false;

                                try
                                {
                                    foreach (var t in assembly.GetTypes() )
                                    {
                                        foreach (var a in t.GetCustomAttributes<TraitClassAttribute>() )
                                        {
                                            if (a.Name == Trait.ClassName)
                                            {
                                                type = t;
                                            
                                                found = true;

                                                break;
                                            }
                                        }

                                        if (found)
                                        {
                                            break;
                                        }
                                    }
                        
                                    if (found)
                                    {
                                        break;
                                    }
                                }
                                catch { }
                            }

                            if (type == null)
                            {
                                throw new Exception("Can not instanciate class " + Trait.ClassName);
                            }
                            else
                            {
                                var instance = Activator.CreateInstance(type);

                                toObject = instance;

                                if (Trait.Members.Count == Values.Count)
                                {
                                    for (int i = 0; i < Trait.Members.Count; i++)
                                    {
                                        var property = type.GetProperties()
                                                           .Where(p => p.GetCustomAttributes<TraitMemberAttribute>()
                                                                        .Where(a => a.Name == Trait.Members[i] )
                                                                        .Any() )
                                                           .FirstOrDefault();

                                        if (property == null)
                                        {
                                            throw new Exception("Can not find property " + Trait.Members[i] + " of class " + Trait.ClassName);
                                        }
                                        else
                                        {
                                            var value = Values[i];

                                            if (value is Amf3Object)
                                            {
                                                value = ( (Amf3Object)value ).ToObject;
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
}