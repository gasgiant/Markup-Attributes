using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EndGroupAttribute : Attribute
    {
        public string GroupName { get; private set; }

        public EndGroupAttribute(string groupName = null)
        {
            GroupName = groupName;
        }
    }
}
