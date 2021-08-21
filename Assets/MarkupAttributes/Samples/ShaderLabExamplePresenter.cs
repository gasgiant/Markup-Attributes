using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ShaderLabExamplePresenter : SamplesBehaviour
    {
        [Space]
        public Shader shader;

        [InlineEditor(stripped : true)]
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
