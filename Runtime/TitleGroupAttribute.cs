using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleGroupAttribute : LayoutGroupAttribute
    {
        public TitleGroupAttribute(string path, bool contentBox = false)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Label;
            BodyStyle = contentBox ? MarkupBodyStyle.ContentBox : MarkupBodyStyle.SeparatorLine;
        }
    }
}
