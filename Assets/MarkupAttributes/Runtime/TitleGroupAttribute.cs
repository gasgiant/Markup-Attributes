using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleGroupAttribute : LayoutGroupAttribute
    {
        public TitleGroupAttribute(string path, bool boxContent = false)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Label;
            BodyStyle = boxContent ? MarkupBodyStyle.ContentBox : MarkupBodyStyle.SeparatorLine;
        }
    }
}
