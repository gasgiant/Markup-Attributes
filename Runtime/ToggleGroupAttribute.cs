using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ToggleGroupAttribute : LayoutGroupAttribute
    {
        public ToggleGroupAttribute(string path, bool foldable = false, float space = 0)
        {
            Path = path;
            Toggle = true;
            Type = LayoutGroupType.Vertical;
            HeaderFlags = MarkupHeaderFlags.Label;
            if (foldable)
                HeaderFlags |= MarkupHeaderFlags.Foldable;
            BodyStyle = MarkupBodyStyle.Box;
            Space = space;
        }

        public static ToggleGroupAttribute CreateForShader(string path, bool foldable, string shaderKeyword)
        {
            var value = new ToggleGroupAttribute(path, foldable);
            value.ToggleShaderKeyword = shaderKeyword;
            return value;
        }
    }
}

