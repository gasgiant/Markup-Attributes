using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class FoldoutAttribute : LayoutGroupAttribute
    {
        public FoldoutAttribute(string path, MarkupBodyStyle bodyStyle = MarkupBodyStyle.OutlinedBox)
        {
            Path = path;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Label | MarkupHeaderStyle.Foldable;
            if (bodyStyle != MarkupBodyStyle.OutlinedBox)
                HeaderStyle |= MarkupHeaderStyle.Underline;
            BodyStyle = bodyStyle;
        }
    }
}
