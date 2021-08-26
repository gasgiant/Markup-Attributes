using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : Attribute
    {
        public ConditionDescriptor Condition { get; protected set; }

        public HideIfAttribute(string condition)
        {
            Condition = new ConditionDescriptor(condition, false);
        }

        public HideIfAttribute(string condition, int enumValue)
        {
            Condition = new ConditionDescriptor(condition, false, enumValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : HideIfAttribute
    {
        public ShowIfAttribute(string condition) : base(condition)
        {
            Condition.isInverted = true;
        }

        public ShowIfAttribute(string condition, int enumValue) : base(condition, enumValue)
        {
            Condition.isInverted = true;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : Attribute
    {
        public ConditionDescriptor Condition { get; protected set; }

        public DisableIfAttribute(string condition)
        {
            Condition = new ConditionDescriptor(condition, false);
        }

        public DisableIfAttribute(string condition, int enumValue)
        {
            Condition = new ConditionDescriptor(condition, false, enumValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfAttribute : DisableIfAttribute
    {
        public EnableIfAttribute(string condition) : base(condition)
        {
            Condition.isInverted = true;
        }

        public EnableIfAttribute(string condition, int enumValue) : base(condition, enumValue)
        {
            Condition.isInverted = true;
        }
    }
}


