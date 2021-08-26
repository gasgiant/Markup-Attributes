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
            Condition = new ConditionDescriptor(condition, false);
        }

        public HideIfGroupAttribute(string path, string condition, int enumValue)
        {
            Path = path;
            Type = LayoutGroupType.HideIf;
            Condition = new ConditionDescriptor(condition, false, enumValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfGroupAttribute : HideIfGroupAttribute
    {
        public ShowIfGroupAttribute(string path, string condition) : base(path, condition)
        {
            Condition.isInverted = true;
        }

        public ShowIfGroupAttribute(string path, string condition, int enumValue) : base(path, condition, enumValue)
        {
            Condition.isInverted = true;
        }
    }
}

