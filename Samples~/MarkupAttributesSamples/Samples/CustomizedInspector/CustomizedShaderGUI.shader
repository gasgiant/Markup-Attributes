Shader "MarkupAttributes/CustomizedShaderGUI"
{
    Properties
    {
        [HorizontalGroup(Split, 50)]
        [Box(. Left, true)]
        _One("One", Float) = 0
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0

        [Box(.. Right, true)]
        _Four("Four", Float) = 0
        _Five("Five", Float) = 0
        _Six("Six", Float) = 0
    }

    CustomEditor "MarkupAttributes.Samples.CustomizedShaderGUI"

    SubShader
    {
        Pass
        {
        }
    }
}
