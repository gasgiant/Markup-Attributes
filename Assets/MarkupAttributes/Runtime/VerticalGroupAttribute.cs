using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class VerticalGroupAttribute : LayoutGroupAttribute
    {
        public VerticalGroupAttribute(string path)
        {
            Path = path;
            Type = GroupType.Vertical;
        }
    }
}
