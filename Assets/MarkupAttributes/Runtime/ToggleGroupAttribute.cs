using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ToggleGroupAttribute : LayoutGroupAttribute
    {
        public ToggleGroupAttribute(string path, bool foldable = false)
        {
            Path = path;
            Toggle = true;
            Type = LayoutGroupType.Vertical;
            HeaderStyle = MarkupHeaderStyle.Label;
            if (foldable)
                HeaderStyle |= MarkupHeaderStyle.Foldable;
            BodyStyle = MarkupBodyStyle.OutlinedBox;
        }

        public static ToggleGroupAttribute CreateForShader(string path, bool foldable, string shaderKeyword)
        {
            var value = new ToggleGroupAttribute(path, foldable);
            value.ShaderKeyword = shaderKeyword;
            return value;
        }
    }
}

