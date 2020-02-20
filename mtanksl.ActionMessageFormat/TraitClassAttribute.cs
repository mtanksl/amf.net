using System;

namespace mtanksl.ActionMessageFormat
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]

    public sealed class TraitClassAttribute : Attribute
    {
        public TraitClassAttribute(string name)
        {
            this.name = name;
        }

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}