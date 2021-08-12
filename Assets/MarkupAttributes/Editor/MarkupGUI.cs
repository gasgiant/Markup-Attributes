using System;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public static class MarkupGUI
    {
        internal const float SpaceAfterBoxedHeader = 2;
        internal const float SpaceAfterGroup = 5;

        public static bool DrawScriptProperty => drawScriptProperty;
        private static bool drawScriptProperty = true;

        public struct DrawScriptPropertyScope : IDisposable
        {
            private readonly bool cachedDrawScriptProperty;

            public DrawScriptPropertyScope(bool draw)
            {
                cachedDrawScriptProperty = drawScriptProperty;
                drawScriptProperty &= draw;
            }

            public void Dispose()
            {
                drawScriptProperty = cachedDrawScriptProperty;
            }
        }

        public struct SetIndentLevelScope : IDisposable
        {
            private readonly int cachedIndentLevel;

            public SetIndentLevelScope(int indentLevel)
            {
                cachedIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel;
            }

            public void Dispose()
            {
                EditorGUI.indentLevel = cachedIndentLevel;
            }
        }

        internal static void DrawEditorInline(SerializedProperty property, 
            UnityEditor.Editor editor, bool stripped = false, bool enabled = true)
        {
            stripped &= editor != null;
            bool expanded = property.isExpanded && editor != null && !property.hasMultipleDifferentValues;
            bool hierarchyMode = EditorGUIUtility.hierarchyMode;
            float labelWidth = EditorGUIUtility.labelWidth;
            MaterialEditor materialEditor = editor as MaterialEditor;
            if (!stripped)
            {
                EditorGUILayout.BeginVertical(MarkupStyles.GroupBox);
                EditorGUIUtility.hierarchyMode = false;
                EditorGUIUtility.labelWidth = labelWidth;

                Rect headerRect = HeaderBase(GroupStyle.Box, expanded);
                property.isExpanded = FoldoutWithObjectField(headerRect, property);
                if (expanded)
                    EditorGUILayout.Space(SpaceAfterBoxedHeader);
            }

            if (expanded || stripped)
            {
                // begin dummy vertical group,
                // because MaterialEditor interrupts one in OnInspectorGUI
                if (materialEditor)
                    EditorGUILayout.BeginVertical();
                using (new EditorGUI.DisabledScope(!enabled))
                {
                    using (new DrawScriptPropertyScope(false))
                    {
                        if (materialEditor)
                            editor.DrawHeader();
                        editor.OnInspectorGUI();
                    }
                }
                if (materialEditor)
                    EditorGUILayout.EndVertical();
            }

            if (!stripped)
            {
                if (expanded)
                    EditorGUILayout.Space(SpaceAfterBoxedHeader);
                EditorGUILayout.EndVertical();
                EditorGUIUtility.hierarchyMode = hierarchyMode;
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        internal static Rect HeaderBase(GroupStyle style, bool expanded)
        {
            bool box = style.HasFlag(GroupStyle.Box);
            bool line = style.HasFlag(GroupStyle.LabelUnderline);
            var padding = MarkupStyles.GroupBox.padding;

            if (!box)
                EditorGUILayout.Space(SpaceAfterBoxedHeader);

            Rect rect = EditorGUILayout.GetControlRect();
            if (box)
            {
                Rect boxRect = padding.Add(rect);
                GUI.Box(boxRect, GUIContent.none, MarkupStyles.HeaderBox(expanded));
            }

            if (expanded)
            {
                if (box)
                    EditorGUILayout.Space(SpaceAfterBoxedHeader);
                if (line)
                    HorizontalLine();
            }
            return rect;
        }

        internal static void HorizontalLine(float height = 1)
        {
            float c = EditorGUIUtility.isProSkin ? 0.5f : 0.4f;
            HorizontalLine(new Color(c, c, c), height);
        }

        internal static void HorizontalLine(Color color, float height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color);
            EditorGUILayout.GetControlRect(false, height);
        }

        internal static bool FoldoutWithObjectField(SerializedProperty property)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            bool res = FoldoutWithObjectField(rect, property);
            return res;
        }

        internal static bool FoldoutWithObjectField(Rect position, 
            SerializedProperty property, GUIContent label = null)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            bool result = false;

            if (property.objectReferenceValue != null)
                result = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            else
                EditorGUI.LabelField(position, label);

            float xOffset = EditorGUIUtility.labelWidth - 0.5f * MarkupStyles.GroupBox.padding.left;
            var propertyRect = new Rect(position.x + xOffset,
                position.y, position.width - xOffset, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(propertyRect, property, GUIContent.none);
            EditorGUI.EndProperty();
            return result;
        }

        internal static bool ToggleGroupHeader(Rect position,
            TogglableValueWrapper wrapper, GUIContent label, ref bool isExpanded, bool foldable)
        {
            float toggleWidth = EditorStyles.boldLabel.CalcSize(label).x + 20;
            Rect rectToggle = position;
            rectToggle.width = toggleWidth;
            Rect rectFoldout = position;
            rectFoldout.x += toggleWidth;
            rectFoldout.width -= toggleWidth;

            bool hasMixedValue = wrapper.HasMixedValue;
            bool value = wrapper.GetValue();
            if (wrapper.TargetSerializedProperty != null)
                EditorGUI.BeginProperty(position, label, wrapper.TargetSerializedProperty);
            else
                EditorGUI.showMixedValue = hasMixedValue;
            
            EditorGUI.BeginChangeCheck();
            
            value = EditorGUI.ToggleLeft(rectToggle, label, value, EditorStyles.boldLabel);
            if (EditorGUI.EndChangeCheck())
                wrapper.SetValue(value, true);
            

            if (foldable)
            {
                Rect smallRect = rectFoldout;
                smallRect.x += rectFoldout.width - 20;
                smallRect.width = 20;
                isExpanded = EditorGUI.Foldout(smallRect, isExpanded, GUIContent.none);

                Rect controlRect = rectFoldout;
                var padding = MarkupStyles.GroupBox.padding;
                controlRect.y -= padding.top;
                controlRect.height += padding.top + padding.bottom;
                isExpanded = EditorGUI.Foldout(controlRect, isExpanded, 
                    GUIContent.none, true, EditorStyles.label);
            }

            if (wrapper.TargetSerializedProperty != null)
                EditorGUI.EndProperty();
            else
                EditorGUI.showMixedValue = false;

            if (!foldable)
                isExpanded = value;
            return value && !hasMixedValue;
        }

        internal static int TabsControl(int selected, string[] tabs, bool isBoxed)
        {
            if (isBoxed)
            {
                Rect position = EditorGUILayout.GetControlRect(false);
                var padding = MarkupStyles.GroupBox.padding;
                position.x -= padding.left;
                position.width += padding.right + padding.left;
                position.width -= 1;
                position.y -= padding.top;
                position.height += padding.top;

                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.alignment = TextAnchor.MiddleCenter;

                for (int i = 0; i < tabs.Length; i++)
                {
                    Rect r = GetTabRect(position, i, tabs.Length, out GUIStyle style);
                    if (EditorGUI.Toggle(r, i == selected, style))
                        selected = i;
                    EditorGUI.LabelField(r, tabs[i], labelStyle);
                }
            }
            else
                selected = GUILayout.Toolbar(selected, tabs);
            EditorGUILayout.Space(SpaceAfterBoxedHeader);

            return selected;
        } 

        private static GUIStyle s_TabOnlyOne;
        private static GUIStyle s_TabFirst;
        private static GUIStyle s_TabMiddle;
        private static GUIStyle s_TabLast;
        private static Rect GetTabRect(Rect rect, int tabIndex, int tabCount, out GUIStyle tabStyle)
        {
            if (s_TabOnlyOne == null)
            {
                s_TabOnlyOne = "Tab onlyOne";
                s_TabFirst = "Tab first";
                s_TabMiddle = "Tab middle";
                s_TabLast = "Tab last";
            }

            tabStyle = s_TabMiddle;

            if (tabCount == 1)
            {
                tabStyle = s_TabOnlyOne;
            }
            else if (tabIndex == 0)
            {
                tabStyle = s_TabFirst;
            }
            else if (tabIndex == (tabCount - 1))
            {
                tabStyle = s_TabLast;
            }

            float tabWidth = rect.width / tabCount;
            int left = Mathf.RoundToInt(tabIndex * tabWidth);
            int right = Mathf.RoundToInt((tabIndex + 1) * tabWidth);
            return new Rect(rect.x + left, rect.y, right - left, rect.height);
        }
    }
}
