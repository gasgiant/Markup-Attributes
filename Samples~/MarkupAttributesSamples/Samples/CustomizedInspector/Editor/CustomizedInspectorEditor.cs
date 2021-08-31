using UnityEditor;
using UnityEngine;
using MarkupAttributes.Editor;

namespace MarkupAttributes.Samples
{
    [CustomEditor(typeof(CustomizedInspector))]
    public class CustomizedInspectorEditor : MarkedUpEditor
    {
        protected override void OnInitialize()
        {
            AddCallback(serializedObject.FindProperty("one"),
                CallbackEvent.AfterProperty, ButtonAfterOne);

            AddCallback(serializedObject.FindProperty("six"),
                CallbackEvent.BeforeProperty, ButtonBeforeSix);

            AddCallback(serializedObject.FindProperty("three"),
                CallbackEvent.ReplaceProperty, ButtonReplaceThree);
        }

        private void ButtonAfterOne(SerializedProperty property)
        {
            GUILayout.Button("After One");
        }

        private void ButtonBeforeSix(SerializedProperty property)
        {
            GUILayout.Button("Before Six");
        }

        private void ButtonReplaceThree(SerializedProperty property)
        {
            GUILayout.Button("Replace Three");
        }
    }
}