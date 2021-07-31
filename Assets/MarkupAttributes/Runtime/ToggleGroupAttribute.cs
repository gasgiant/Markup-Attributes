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
            Type = GroupType.Vertical;
            Style = GroupStyle.Box | GroupStyle.Label;
            if (foldable)
                Style |= GroupStyle.Foldable;
        }

        public static ToggleGroupAttribute CreateForShader(string path, bool foldable, string shaderKeyword)
        {
            var value = new ToggleGroupAttribute(path, foldable);
            value.ShaderKeyword = shaderKeyword;
            return value;
        }
    }
}

