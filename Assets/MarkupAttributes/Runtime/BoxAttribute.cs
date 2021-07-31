using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BoxAttribute : LayoutGroupAttribute
    {
        public BoxAttribute(string path, bool labeled = false)
        {
            Path = path;
            Type = GroupType.Vertical;
            Style = GroupStyle.Box;
            if (labeled)
                Style |= GroupStyle.Label;
        }
    }
}
