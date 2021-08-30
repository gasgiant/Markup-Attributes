using UnityEditor;
using MarkupAttributes.Editor;

namespace MarkupAttributes.Samples
{
    public class CustomWindow : EditorWindow
    {
        MarkupGUI.GroupsStack groupsStack = new MarkupGUI.GroupsStack();
        int activeTab = 0;
        bool foldout = true;

        public static void Open()
        {
            CustomWindow window = (CustomWindow)GetWindow(typeof(CustomWindow), 
                false, "Custom Window Sample");
            window.Show();
        }

        void OnGUI()
        {
            // Always clear the groups stack.
            groupsStack.Clear();

            int k = 0;

            groupsStack += MarkupGUI.BeginBoxGroup("Box");
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
            k += activeTab * 3;
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.IntField("Int " + k, 0);
                k++;
            }

            // Always end all groups at the end.
            groupsStack.EndAll();
        }
    }
}
