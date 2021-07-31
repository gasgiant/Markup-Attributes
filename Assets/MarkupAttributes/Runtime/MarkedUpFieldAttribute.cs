using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MarkedUpFieldAttribute : Attribute
    {
        public bool showControl;
        public bool indentChildren;

        public MarkedUpFieldAttribute(bool showControl = true, bool indentChildren = true)
        {
            this.showControl = showControl;
            this.indentChildren = indentChildren;
        }
    }
}
