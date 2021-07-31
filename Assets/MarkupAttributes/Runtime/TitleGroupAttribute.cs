using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleGroupAttribute : LayoutGroupAttribute
    {
        public TitleGroupAttribute(string path)
        {
            Path = path;
            Type = GroupType.Vertical;
            Style = GroupStyle.Label | GroupStyle.LabelUnderline;
        }
    }
}
