using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class InlineEditorSample : SamplesBehaviour
    {
        [InlineEditor]
        public SomeData someData;

        [InlineEditor(InlineEditorMode.Box)]
        public SomeComponent someComponent;
        
        [InlineEditor(InlineEditorMode.Box)]
        public Material material;

        [TitleGroup("Stripped")]
        [InlineEditor(InlineEditorMode.Stripped)]
        public SomeData stripped1;
    }
}
