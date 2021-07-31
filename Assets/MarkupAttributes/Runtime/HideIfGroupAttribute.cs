using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfGroupAttribute : LayoutGroupAttribute
    {
        public HideIfGroupAttribute(string path, string condition)
        {
            Path = path;
            Type = GroupType.HideIf;
            Condition = condition;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfGroupAttribute : HideIfGroupAttribute
    {
        public ShowIfGroupAttribute(string path, string condition) : base(path, condition)
        {
            ConditionInverted = true;
        }
    }
}

