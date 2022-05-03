using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BoxAttribute : LayoutGroupAttribute
    {
        public BoxAttribute(string path, bool labeled = true, float space = 0)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            if (labeled)
                HeaderFlags = MarkupHeaderFlags.Label;
            BodyStyle = MarkupBodyStyle.Box;
            Space = space;
        }
    }
}
