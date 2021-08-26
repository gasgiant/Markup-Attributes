namespace MarkupAttributes
{
    public class ConditionDescriptor
    {
        public bool? fixedValue;
        public string memberName;
        public bool isInverted;
        public object value;
        public bool hasValue;

        internal ConditionDescriptor(string memberName, bool isInverted)
        {
            this.memberName = memberName;
            this.isInverted = isInverted;
        }

        internal ConditionDescriptor(string memberName, bool isInverted, object value)
        {
            this.memberName = memberName;
            this.isInverted = isInverted;
            this.value = value;
            hasValue = true;
        }

        internal ConditionDescriptor(bool fixedValue)
        {
            this.fixedValue = fixedValue;
        }
    }
}
