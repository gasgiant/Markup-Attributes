using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InlineEditorAttribute : Attribute
    {
        public InlineEditorMode Mode { get; private set; }

        public InlineEditorAttribute(InlineEditorMode mode = InlineEditorMode.Box)
        {
            Mode = mode;
        }
    }

    public enum InlineEditorMode { Box, ContentBox, Stripped }
}
