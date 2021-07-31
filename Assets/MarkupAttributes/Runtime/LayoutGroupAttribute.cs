using System;

namespace MarkupAttributes
{
    public abstract class LayoutGroupAttribute : Attribute
    {
        public string Path { get; protected set; }
        public GroupType Type { get; protected set; }
        public GroupStyle Style { get; protected set; }
        public bool Toggle { get; protected set; }
        public float LabelWidth { get; protected set; }
        public string[] Tabs { get; protected set; }
        public string Condition { get; protected set; }
        public bool ConditionInverted { get; protected set; }
        public string ShaderKeyword { get; protected set; }

        public bool NeedsCondition => Type == GroupType.DisableIf
            || Type == GroupType.HideIf;
    }

    public enum GroupType
    {
        LocalScope,
        Vertical,
        Horizontal,
        TabScope,
        Tab,
        DisableIf,
        HideIf
    }

    [Flags]
    public enum GroupStyle
    {
        None = 0,
        Label = 1,
        Box = 2,
        Foldable = 4,
        LabelUnderline = 8,

        Foldout = Label | Foldable
    }
}



