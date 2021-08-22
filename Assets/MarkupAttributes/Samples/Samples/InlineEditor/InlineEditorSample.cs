using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class InlineEditorSample : SamplesBehaviour
    {
        [InlineEditor]
        [SerializeField] private SomeData someData;

        [InlineEditor(InlineEditorMode.Box)]
        [SerializeField] private SomeComponent someComponent;
        
        [InlineEditor(InlineEditorMode.Box)]
        [SerializeField] private Material material;

        [TitleGroup("Stripped")]
        [InlineEditor(InlineEditorMode.Stripped)]
        [SerializeField] private SomeData stripped1;
    }
}
