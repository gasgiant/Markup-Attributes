using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleGroupAttribute : LayoutGroupAttribute
    {
        public TitleGroupAttribute(string path, bool contentBox = false, bool underline = true, float space = 3)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderFlags = MarkupHeaderFlags.Label;
            if (underline)
                HeaderFlags |= MarkupHeaderFlags.Underline;
            BodyStyle = contentBox ? MarkupBodyStyle.ContentBox : MarkupBodyStyle.None;
            Space = space;
        }
    }
}
