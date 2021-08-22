using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InlineEditorAttribute : Attribute
    {
        public InlineEditorMode mode;

        public InlineEditorAttribute(InlineEditorMode mode = InlineEditorMode.ContentBox)
        {
            this.mode = mode;
        }
    }

    public enum InlineEditorMode { Box, ContentBox, Stripped }
}
