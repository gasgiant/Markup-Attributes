using UnityEditor;
using UnityEngine;
using MarkupAttributes.Editor;

namespace MarkupAttributes.Samples
{
    public class CustomWindow : EditorWindow
    {
        [MenuItem("Window/CustomWindow")]
        static void Init()
        {
            CustomWindow window = (CustomWindow)GetWindow(typeof(CustomWindow));
            window.name = "Custom Window Sample";
            window.Show();
        }

        void OnGUI()
        {
            //MarkupGUI.BeginBoxGroup(new GUIContent("Ints"));
            //for (int i = 0; i < 3; i++)
            //{
            //    EditorGUILayout.IntField("Int " + i, 0);
            //}
            //EditorGUILayout.EndVertical();
        }
    }
}
