using System;

namespace MarkupAttributes
{
    public class ConditionDescriptor
    {
        public string condition;
        public bool isInverted;
        public int? enumValue = null;

        internal ConditionDescriptor(string condition, bool isInverted)
        {
            this.condition = condition;
            this.isInverted = isInverted;
        }

        internal ConditionDescriptor(string condition, bool isInverted, int enumValue)
        {
            this.condition = condition;
            this.isInverted = isInverted;
            this.enumValue = enumValue;
        }
    }
}
