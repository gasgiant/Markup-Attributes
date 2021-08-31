using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfGroupAttribute : LayoutGroupAttribute
    {
        public HideIfGroupAttribute(string path, string memberName)
        {
            Path = path;
            Type = LayoutGroupType.HideIf;
            Condition = new ConditionDescriptor(memberName, false);
        }

        public HideIfGroupAttribute(string path, string memberName, object value)
        {
            Path = path;
            Type = LayoutGroupType.HideIf;
            Condition = new ConditionDescriptor(memberName, false, value);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfGroupAttribute : HideIfGroupAttribute
    {
        public ShowIfGroupAttribute(string path, string memberName) : base(path, memberName)
        {
            Condition.isInverted = true;
        }

        public ShowIfGroupAttribute(string path, string memberName, object value) : base(path, memberName, value)
        {
            Condition.isInverted = true;
        }
    }
}

