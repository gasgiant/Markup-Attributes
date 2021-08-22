using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class FoldoutAttribute : LayoutGroupAttribute
    {
        public FoldoutAttribute(string path, bool boxed = true)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Foldable;
            BodyStyle = boxed ? MarkupBodyStyle.Box : MarkupBodyStyle.ContentBox;
        }
    }
}
