using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class FoldoutAttribute : LayoutGroupAttribute
    {
        public FoldoutAttribute(string path, bool boxed = true)
        {
            Path = path;
            Type = GroupType.Vertical;
            Style = GroupStyle.Foldout;
            if (boxed)
                Style |= GroupStyle.Box;
            else
                Style |= GroupStyle.LabelUnderline;
        }
    }
}
