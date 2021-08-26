using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MarkedUpFieldAttribute : Attribute
    {
        public bool ShowControl { get; private set; }
        public bool IndentChildren { get; private set; }

        public MarkedUpFieldAttribute(bool showControl = true, bool indentChildren = true)
        {
            ShowControl = showControl;
            IndentChildren = indentChildren;
        }
    }
}
