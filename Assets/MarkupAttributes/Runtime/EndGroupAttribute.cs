using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EndGroupAttribute : Attribute
    {
        public string name;

        public EndGroupAttribute(string name = null)
        {
            this.name = name;
        }
    }
}
