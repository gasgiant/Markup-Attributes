using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ShaderLabExamplePresenter : SamplesBehaviour
    {
        [ReadOnly]
        public Shader shader;

        [InlineEditor(InlineEditorMode.Stripped)]
        public Material material;

        private void OnValidate()
        {
            if (material)
                shader = material.shader;
            else
                shader = null;
        }
    }
}
