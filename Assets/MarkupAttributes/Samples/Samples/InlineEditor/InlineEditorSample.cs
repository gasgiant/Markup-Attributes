using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class InlineEditorSample : SamplesBehaviour
    {
        [InlineEditor]
        [SerializeField] private SomeData someData;

        [InlineEditor]
        [SerializeField] private SomeComponent someComponent;
        
        [InlineEditor]
        [SerializeField] private Material material;

        [TitleGroup("Stripped")]
        [InlineEditor(stripped: true)]
        [SerializeField] private SomeData stripped;
    }
}
