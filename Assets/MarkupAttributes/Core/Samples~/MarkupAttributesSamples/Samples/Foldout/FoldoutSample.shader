Shader "MarkupAttributes/FoldoutSample"
{
    Properties
    {
        [Foldout(Foldout_In_A_Box)]
        _One("One", Float) = 0
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0

        [Foldout(Foldout, false)]
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
