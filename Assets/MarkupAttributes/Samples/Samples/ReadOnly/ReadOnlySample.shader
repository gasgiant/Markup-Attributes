Shader "MarkupAttributes/ReadOnlySample"
{
    Properties
    {
        [ReadOnly]
        _One("One", Float) = 0

        [ReadOnlyGroup(Read_Only_Group)]
        [Header(Read Only Group)]
        [Space]
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0
        _Four("Four", Float) = 0
    }

    CustomEditor "MarkupAttributes.Editor.MarkedUpShaderGUI"

    SubShader
    {
        Pass
        {
        }
    }
}
