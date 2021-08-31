Shader "MarkupAttributes/TitleGroup"
{
    Properties
    {
        [TitleGroup(Title_Group)]
        _One("One", Float) = 0
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0

        [TitleGroup(Title_Group_With_A_Box, true)]
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
