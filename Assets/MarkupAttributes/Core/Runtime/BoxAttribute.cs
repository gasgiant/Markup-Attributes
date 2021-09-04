using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BoxAttribute : LayoutGroupAttribute
    {
        public BoxAttribute(string path, bool labeled = true)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            if (labeled)
                HeaderFlags = MarkupHeaderFlags.Label;
            BodyStyle = MarkupBodyStyle.Box;
        }
    }
}
