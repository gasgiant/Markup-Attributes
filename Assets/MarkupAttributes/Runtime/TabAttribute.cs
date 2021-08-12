using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TabAttribute : LayoutGroupAttribute
    {
        public TabAttribute(string path)
        {
            Path = path;
            Type = LayoutGroupType.Tab;
        }
    }
}
