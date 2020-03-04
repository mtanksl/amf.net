using System;
using System.Reflection;

namespace mtanksl.ActionMessageFormat
{
    public static class Util
    {
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