using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class FoldoutAttribute : LayoutGroupAttribute
    {
        public FoldoutAttribute(string path, bool box = true, float space = 0)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderFlags = MarkupHeaderFlags.Label | MarkupHeaderFlags.Foldable;
            BodyStyle = box ? MarkupBodyStyle.Box : MarkupBodyStyle.ContentBox;
            Space = space;
        }
    }
}
