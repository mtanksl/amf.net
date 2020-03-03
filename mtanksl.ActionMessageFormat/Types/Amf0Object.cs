using System;
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

            return "Class " + ClassName;
        }

        public void Read(object value)
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

        public object ToObject
        {
            get
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
                                            if (a.Name == ClassName)
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
                                throw new Exception("Can not instanciate class " + ClassName);
                            }
                            else
                            {
                                var instance = Activator.CreateInstance(type);

                                toObject = instance;

                                foreach (var pair in DynamicMembersAndValues)
                                {
                                    var property = type.GetProperties()
                                                       .Where(p => p.GetCustomAttributes<TraitMemberAttribute>()
                                                                    .Where(a => a.Name == pair.Key)
                                                                    .Any() )
                                                       .FirstOrDefault();

                                    if (property == null)
                                    {
                                        throw new Exception("Can not find property " + pair.Key + " of class " + ClassName);
                                    }
                                    else
                                    { 
                                        var value = pair.Value;

                                        if (value is Amf0Object)
                                        {
                                            value = ( (Amf0Object)value ).ToObject;
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