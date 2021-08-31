using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : Attribute
    {
        public ConditionDescriptor Condition { get; protected set; }

        public HideIfAttribute(string memberName)
        {
            Condition = new ConditionDescriptor(memberName, false);
        }

        public HideIfAttribute(string memberName, object value)
        {
            Condition = new ConditionDescriptor(memberName, false, value);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : HideIfAttribute
    {
        public ShowIfAttribute(string memberName) : base(memberName)
        {
            Condition.isInverted = true;
        }

        public ShowIfAttribute(string memberName, object value) : base(memberName, value)
        {
            Condition.isInverted = true;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : Attribute
    {
        public ConditionDescriptor Condition { get; protected set; }

        public DisableIfAttribute(string memberName)
        {
            Condition = new ConditionDescriptor(memberName, false);
        }

        public DisableIfAttribute(string memberName, object value)
        {
            Condition = new ConditionDescriptor(memberName, false, value);
        }

        protected DisableIfAttribute(bool fixedValue)
        {
            Condition = new ConditionDescriptor(fixedValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfAttribute : DisableIfAttribute
    {
        public EnableIfAttribute(string memberName) : base(memberName)
        {
            Condition.isInverted = true;
        }

        public EnableIfAttribute(string memberName, object value) : base(memberName, value)
        {
            Condition.isInverted = true;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyAttribute : DisableIfAttribute
    {
        public ReadOnlyAttribute() : base(true)
        {
        }
    }
}


