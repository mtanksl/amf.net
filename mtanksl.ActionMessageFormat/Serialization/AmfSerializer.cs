using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace mtanksl.ActionMessageFormat
{
    public class AmfSerializer
    {
        public static AmfSerializer Default
        {
            get
            {
                return new AmfSerializer() { ThrowIfPropertyNotFound = false };
            }
        }

        public bool ThrowIfPropertyNotFound { get; set; }

        public object Normalize(object value)
        {
            if (value is IAmfObject)
            {
                return ( (IAmfObject)value ).ToObject(this);
            }

            if (value is List<object>)
            {
                return ( (List<object>)value ).Select(i => Normalize(i) ).ToList();
            }

            if (value is Dictionary<string, object>)
            {
                return ( (Dictionary<string, object>)value ).Select(i => new { Key = i.Key, Value = Normalize(i.Value) } ).ToDictionary(i => i.Key, i => i.Value);
            }

            if (value is Dictionary<object, object>)
            {
                return ( (Dictionary<object, object>)value ).Select(i => new { Key = Normalize(i.Key), Value = Normalize(i.Value) } ).ToDictionary(i => i.Key, i => i.Value);
            }

            return value;
        }

        public static Type GetTypeByTraitClassName(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                try
                {
                    foreach (var type in assembly.GetTypes() )
                    {
                        foreach (var attribute in type.GetCustomAttributes<TraitClassAttribute>() )
                        {
                            if (attribute.Name == className)
                            {
                                return type;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }
    }
}