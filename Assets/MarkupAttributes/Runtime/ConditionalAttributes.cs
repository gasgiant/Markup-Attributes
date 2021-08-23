using System;

namespace MarkupAttributes
{
    public abstract class ConditionalAttribute : Attribute
    {
        public string Condition { get; protected set; }
        public bool IsInverted { get; protected set; } = false;
    }

    public class HideIfAttribute : ConditionalAttribute
    {
        public HideIfAttribute(string condition)
        {
            Condition = condition;
        }
    }

    public class ShowIfAttribute : HideIfAttribute
    {
        public ShowIfAttribute(string condition) : base(condition)
        {
            IsInverted = true;
        }
    }

    public class DisableIfAttribute : ConditionalAttribute
    {
        public DisableIfAttribute(string condition)
        {
            Condition = condition;
        }
    }

    public class EnableIfAttribute : DisableIfAttribute
    {
        public EnableIfAttribute(string condition) : base(condition)
        {
            IsInverted = true;
        }
    }
}


