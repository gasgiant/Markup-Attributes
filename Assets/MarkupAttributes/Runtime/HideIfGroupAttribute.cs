using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfGroupAttribute : LayoutGroupAttribute
    {
        public HideIfGroupAttribute(string path, string condition)
        {
            Path = path;
            Type = LayoutGroupType.HideIf;
            Condition = condition;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfGroupAttribute : HideIfGroupAttribute
    {
        public ShowIfGroupAttribute(string path, string condition) : base(path, condition)
        {
            IsConditionInverted = true;
        }
    }
}

