using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class FoldoutAttribute : LayoutGroupAttribute
    {
        public FoldoutAttribute(string path, bool box = true)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Foldable;
            BodyStyle = box ? MarkupBodyStyle.Box : MarkupBodyStyle.ContentBox;
        }
    }
}
