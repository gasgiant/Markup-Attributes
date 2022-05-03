using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public class MarkedUpTypeAttribute : Attribute
    {
        public bool ShowControl { get; protected set; }
        public bool IndentChildren { get; protected set; }

        public MarkedUpTypeAttribute(bool showControl = true, bool indentChildren = true)
        {
            ShowControl = showControl;
            IndentChildren = indentChildren;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MarkedUpFieldAttribute : MarkedUpTypeAttribute
    {
        public MarkedUpFieldAttribute(bool showControl = true, bool indentChildren = true)
        {
            ShowControl = showControl;
            IndentChildren = indentChildren;
        }
    }
}
