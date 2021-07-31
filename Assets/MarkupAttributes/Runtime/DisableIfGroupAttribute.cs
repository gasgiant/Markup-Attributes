using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfGroupAttribute : LayoutGroupAttribute
    {
        public DisableIfGroupAttribute(string path, string condition)
        {
            Path = path;
            Type = GroupType.DisableIf;
            Condition = condition;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfGroupAttribute : DisableIfGroupAttribute
    {
        public EnableIfGroupAttribute(string path, string condition) : base(path, condition)
        {
            ConditionInverted = true;
        }
    }
}

