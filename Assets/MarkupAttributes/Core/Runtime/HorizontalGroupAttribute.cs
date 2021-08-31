using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HorizontalGroupAttribute : LayoutGroupAttribute
    {
        public HorizontalGroupAttribute(string path, float labelWidth)
        {
            Path = path;
            Type = LayoutGroupType.Horizontal;
            LabelWidth = labelWidth;
        }
    }
}
