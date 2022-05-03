Shader "MarkupAttributes/ToggleGroupSample"
{
    Properties
    {
        [ToggleGroup(Toggle_Group)]
        _Toggle("Toggle", Float) = 0
        _One("One", Float) = 0
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0

        [ToggleGroup(Toggle_Group_With_Keyword, true, true, MY_KEYWORD)]
        _AnotherToggle("Another Toggle", Float) = 0
        _Four("Four", Float) = 0
        _Five("Five", Float) = 0
        _Six("Six", Float) = 0
    }

    CustomEditor "MarkupAttributes.Editor.MarkedUpShaderGUI"

    SubShader
    {
        Pass
        {
        }
    }
}
