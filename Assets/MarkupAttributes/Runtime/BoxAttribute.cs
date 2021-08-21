using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BoxAttribute : LayoutGroupAttribute
    {
        public BoxAttribute(string path, bool labeled = false, bool outlined  = true)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            if (labeled)
                HeaderStyle = MarkupHeaderStyle.Label;
            BodyStyle = outlined ? MarkupBodyStyle.FullBox : MarkupBodyStyle.ContentBox;
        }
    }
}
