using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class InlineEditorSample : SamplesBehaviour
    {
        [InlineEditor]
        public SomeData someData;

        [InlineEditor(InlineEditorMode.ContentBox)]
        public SomeComponent someComponent;

        [TitleGroup("Stripped")]
        [InlineEditor(InlineEditorMode.Stripped)]
        public SomeData stripped;
    }
}
