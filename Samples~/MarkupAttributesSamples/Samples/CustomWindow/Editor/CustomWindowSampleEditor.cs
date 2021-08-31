using UnityEngine;
using UnityEditor;

namespace MarkupAttributes.Samples
{
    [CustomEditor(typeof(CustomWindowSample))]
    public class CustomWindowSampleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Open Window"))
            {
                CustomWindow.Open();
            }
        }
    }
}
