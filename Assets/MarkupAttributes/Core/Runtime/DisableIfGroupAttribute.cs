using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfGroupAttribute : LayoutGroupAttribute
    {
        public DisableIfGroupAttribute(string path, string memberName)
        {
            Path = path;
            Type = LayoutGroupType.DisableIf;
            Condition = new ConditionDescriptor(memberName, false);
        }

        public DisableIfGroupAttribute(string path, string memberName, object value)
        {
            Path = path;
            Type = LayoutGroupType.DisableIf;
            Condition = new ConditionDescriptor(memberName, false, value);
        }

        protected DisableIfGroupAttribute(string path, bool fixedValue)
        {
            Path = path;
            Type = LayoutGroupType.DisableIf;
            Condition = new ConditionDescriptor(fixedValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfGroupAttribute : DisableIfGroupAttribute
    {
        public EnableIfGroupAttribute(string path, string memberName) : base(path, memberName)
        {
            Condition.isInverted = true;
        }

        public EnableIfGroupAttribute(string path, string memberName, object value) : base(path, memberName, value)
        {
            Condition.isInverted = true;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyGroupAttribute : DisableIfGroupAttribute
    {
        public ReadOnlyGroupAttribute(string path) : base(path, true)
        {
        }
    }
}

