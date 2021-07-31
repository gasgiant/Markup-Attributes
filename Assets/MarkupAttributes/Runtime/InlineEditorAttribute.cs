using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InlineEditorAttribute : Attribute
    {
        public bool stripped;

        public InlineEditorAttribute(bool stripped = false)
        {
            this.stripped = stripped;
        }
    }
}
