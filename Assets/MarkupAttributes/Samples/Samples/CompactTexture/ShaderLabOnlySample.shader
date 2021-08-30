Shader "MarkupAttributes/ShaderLabOnlySample"
{
    Properties
    {
        [DrawSystemProperties]
        _One("One", Float) = 0
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0
    }

    CustomEditor "MarkupAttributes.Editor.MarkedUpShaderGUI"

    SubShader
    {
        Pass
        {
        }
    }
}
