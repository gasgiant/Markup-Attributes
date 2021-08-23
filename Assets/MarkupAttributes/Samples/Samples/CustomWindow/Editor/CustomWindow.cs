using UnityEditor;
using UnityEngine;
using MarkupAttributes.Editor;

namespace MarkupAttributes.Samples
{
    public class CustomWindow : EditorWindow
    {
        int activeTab = 0;
        bool foldout;

        [MenuItem("Window/CustomWindow")]
        static void Init()
        {
            CustomWindow window = (CustomWindow)GetWindow(typeof(CustomWindow));
            window.name = "Custom Window Sample";
            window.Show();
        }

        void OnGUI()
        {
            int k = 0;
            var groupsStack = new MarkupGUI.GroupsStack();
            groupsStack += MarkupGUI.BeginBoxGroup();
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.IntField("Int " + k, 0);
                k++;
            }
            groupsStack.EndGroup();

            groupsStack += MarkupGUI.BeginTitleGroup("TitleGroup", true);
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.IntField("Int " + k, 0);
                k++;
            }
            groupsStack.EndGroup();

            groupsStack += MarkupGUI.BeginFoldoutGroup(ref foldout, "Foldout");
            for (int i = 0; i < 3; i++)
            {
                if (foldout)
                    EditorGUILayout.IntField("Int " + k, 0);
                k++;
            }
            groupsStack.EndGroup();


            groupsStack += MarkupGUI.BeginTabsGroup(ref activeTab, 
                new string[] { "Left", "Middle", "Right" }, true);
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.IntField("Int " + k, 0);
                k++;
            }
            groupsStack.EndGroup();
        }
    }
}
