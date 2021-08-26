using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class InlineEditorSample : SamplesBehaviour
    {
        [InlineEditor(InlineEditorMode.ContentBox)]
        public SomeComponent someComponent;

        [InlineEditor]
        public SomeData someData;
        
        [InlineEditor]
        public Material material;

        [TitleGroup("Stripped")]
        [InlineEditor(InlineEditorMode.Stripped)]
        public SomeData stripped1;
    }
}
