using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleGroupAttribute : LayoutGroupAttribute
    {
        public TitleGroupAttribute(string path)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Label;
            BodyStyle = MarkupBodyStyle.SeparatorLine;
        }
    }
}
