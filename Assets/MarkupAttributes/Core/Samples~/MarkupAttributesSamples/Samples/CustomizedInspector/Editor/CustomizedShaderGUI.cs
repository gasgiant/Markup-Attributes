using UnityEditor;
using UnityEngine;
using MarkupAttributes.Editor;

namespace MarkupAttributes.Samples
{
    public class CustomizedShaderGUI : MarkedUpShaderGUI
    {
        protected override void OnInitialize(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            AddCallback(FindProperty("_One", properties), CallbackEvent.AfterProperty, ButtonAfterOne);

            AddCallback(FindProperty("_Six", properties), CallbackEvent.BeforeProperty, ButtonBeforeSix);

            AddCallback(FindProperty("_Three", properties), CallbackEvent.ReplaceProperty, ButtonReplaceThree);
        }

        private void ButtonAfterOne(MaterialEditor materialEditor, MaterialProperty[] properties,
            MaterialProperty property)
        {
            GUILayout.Button("After One");
        }

        private void ButtonBeforeSix(MaterialEditor materialEditor, MaterialProperty[] properties,
            MaterialProperty property)
        {
            GUILayout.Button("Before Six");
        }

        private void ButtonReplaceThree(MaterialEditor materialEditor, MaterialProperty[] properties,
            MaterialProperty property)
        {
            GUILayout.Button("Replace Three");
        }
    }
}
