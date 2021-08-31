Shader "MarkupAttributes/TabsSample"
{
    Properties
    {
        [TabScope(Tab_Scope, Left Middle Right, true)]
        [Tab(. Left)]
        _One("One", Float) = 0
        _Two("Two", Float) = 0
        _Three("Three", Float) = 0

        [Tab(.. Middle)]
        _Four("Four", Float) = 0
        _Five("Five", Float) = 0
        _Six("Six", Float) = 0

        [Tab(.. Right)]
        _Seven("Seven", Float) = 0
        _Eight("Eight", Float) = 0
        _Nine("Nine", Float) = 0
    }

    CustomEditor "MarkupAttributes.Editor.MarkedUpShaderGUI"

    SubShader
    {
        Pass
        {
        }
    }
}
