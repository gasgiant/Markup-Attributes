Shader "MarkupAttributes/ConditionalsSample"
{
    Properties
    {
        [MaterialToggle] _Toggle("Toggle", Float) = 0

        [Header(Enabled If Toggle Positive)]
        [Space]
        [EnableIf(_Toggle)]
        _One("One", Float) = 0
        [EnableIf(_Toggle)]
        _Two("Two", Float) = 0

        [Space(10)]
        [Toggle(MY_KEYWORD)] _KeywordToggle("MY_KEYWORD", Float) = 0

        [EnableIfGroup(Keyword, MY_KEYWORD)]
        [Header(Enabled If Keyword)]
        [Space]
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
