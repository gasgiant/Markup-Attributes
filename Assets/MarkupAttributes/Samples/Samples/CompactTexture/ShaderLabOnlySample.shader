Shader "MarkupAttributes/ShaderLabOnlySample"
{
    Properties
    {
        [DrawSystemProperties]
        [CompactTexture]
        _Texture1("Compact Texture", 2D) = "white" {}
        [CompactTexture(ScaleOnly)]
        _Texture2("Scale Only", 2D) = "white" {}
        [CompactTexture(UniformScaleOnly)]
        _Texture3("Uniform Scale", 2D) = "white" {}
    }

    CustomEditor "MarkupAttributes.Editor.MarkedUpShaderGUI"

    SubShader
    {
        Pass
        {
        }
    }
}
