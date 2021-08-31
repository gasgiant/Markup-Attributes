using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    [CustomEditor(typeof(MarkupAttributesPrefs))]
    internal class MarkupAttributesPrefsEditor : UnityEditor.Editor
    {
        string message = "Hi! I am a file that keeps data about " +
            "selected tabs, expanded foldouts and things like that for " +
            "MarkupAttributes.\n" +
            "1. You can keep me inside any of your Editor folders.\n" +
            "2. You can check me out of your version control.";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);

            using (new EditorGUI.DisabledScope(true))
            {
                DrawDefaultInspector();
            }
        }
    }
}
