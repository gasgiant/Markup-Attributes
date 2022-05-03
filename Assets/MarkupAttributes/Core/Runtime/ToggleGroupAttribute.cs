using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ToggleGroupAttribute : LayoutGroupAttribute
    {
        public ToggleGroupAttribute(string path, bool foldable = false, bool box = true, float space = 0)
        {
            Path = path;
            Toggle = true;
            Type = LayoutGroupType.Vertical;
            HeaderFlags = MarkupHeaderFlags.Label;
            if (foldable)
                HeaderFlags |= MarkupHeaderFlags.Foldable;
            BodyStyle = box ? MarkupBodyStyle.Box : MarkupBodyStyle.ContentBox;
            Space = space;
        }

        public static ToggleGroupAttribute CreateForShader(string path, bool foldable, bool box, string shaderKeyword)
        {
            var value = new ToggleGroupAttribute(path, foldable, box);
            value.ToggleShaderKeyword = shaderKeyword;
            return value;
        }
    }
}

