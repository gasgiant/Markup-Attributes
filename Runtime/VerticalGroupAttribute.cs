using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class VerticalGroupAttribute : LayoutGroupAttribute
    {
        public VerticalGroupAttribute(string path, float space = 0)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            Space = space;
        }
    }
}
