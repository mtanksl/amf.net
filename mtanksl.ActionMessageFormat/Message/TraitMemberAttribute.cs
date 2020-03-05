using System;

namespace mtanksl.ActionMessageFormat
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]

    public sealed class TraitMemberAttribute : Attribute
    {
        public TraitMemberAttribute(string name)
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

        public bool Serializable { get; set; } = true;
    }
}