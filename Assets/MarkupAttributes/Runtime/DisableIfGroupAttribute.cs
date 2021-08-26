using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfGroupAttribute : LayoutGroupAttribute
    {
        public DisableIfGroupAttribute(string path, string condition)
        {
            Path = path;
            Type = LayoutGroupType.DisableIf;
            Condition = new ConditionDescriptor(condition, false);
        }

        public DisableIfGroupAttribute(string path, string condition, int enumValue)
        {
            Path = path;
            Type = LayoutGroupType.DisableIf;
            Condition = new ConditionDescriptor(condition, false, enumValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfGroupAttribute : DisableIfGroupAttribute
    {
        public EnableIfGroupAttribute(string path, string condition) : base(path, condition)
        {
            Condition.isInverted = true;
        }

        public EnableIfGroupAttribute(string path, string condition, int enumValue) : base(path, condition, enumValue)
        {
            Condition.isInverted = true;
        }
    }
}

