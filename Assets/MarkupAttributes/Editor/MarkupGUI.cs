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

        public static bool StartNonHierarchyScope()
        {
            bool previous = EditorGUIUtility.hierarchyMode;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.hierarchyMode = false;
            EditorGUIUtility.labelWidth = labelWidth;
            return previous;
        }

        internal static Rect BeginVertical(MarkupBodyStyle style, bool hasHeader, bool isExpanded)
        {
            bool verticalStarted = false;
            Rect headerRect = Rect.zero;

            if (style == MarkupBodyStyle.FullBox)
            {
                EditorGUILayout.BeginVertical(MarkupStyles.OutlinedBox);
                verticalStarted = true;
            }

            if (hasHeader)
            {
                headerRect = EditorGUILayout.GetControlRect();

                if (style == MarkupBodyStyle.FullBox)
                {
                    Rect headerBoxRect = MarkupStyles.OutlinedBox.padding.Add(headerRect);
                    GUI.Box(headerBoxRect, GUIContent.none, MarkupStyles.OutlinedHeaderBox(isExpanded));
                }

                if (isExpanded)
                {
                    if (style == MarkupBodyStyle.FullBox)
                        EditorGUILayout.Space(SpaceAfterBoxedHeader);
                    if (style == MarkupBodyStyle.SeparatorLine)
                        HorizontalLine();
                }
            }

            if (isExpanded && style == MarkupBodyStyle.ContentBox)
            {
                EditorGUILayout.BeginVertical(MarkupStyles.Box);
                verticalStarted = true;
            }

            if (!verticalStarted)
                EditorGUILayout.BeginVertical();

            return headerRect;
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

            float xOffset = EditorGUIUtility.labelWidth - 0.5f * MarkupStyles.OutlinedBox.padding.left;
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
                var padding = MarkupStyles.OutlinedBox.padding;
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
                var padding = MarkupStyles.OutlinedBox.padding;
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
        
        private static Rect GetTabRect(Rect rect, int tabIndex, int tabCount, out GUIStyle tabStyle)
        {
            tabStyle = MarkupStyles.TabMiddle;

            if (tabCount == 1)
            {
                tabStyle = MarkupStyles.TabOnlyOne;
            }
            else if (tabIndex == 0)
            {
                tabStyle = MarkupStyles.TabFirst;
            }
            else if (tabIndex == (tabCount - 1))
            {
                tabStyle = MarkupStyles.TabLast;
            }

            float tabWidth = rect.width / tabCount;
            int left = Mathf.RoundToInt(tabIndex * tabWidth);
            int right = Mathf.RoundToInt((tabIndex + 1) * tabWidth);
            return new Rect(rect.x + left, rect.y, right - left, rect.height);
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
                Rect headerRect = BeginVertical(MarkupBodyStyle.FullBox, true, property.isExpanded);
                StartNonHierarchyScope();
                property.isExpanded = FoldoutWithObjectField(headerRect, property);
                if (expanded)
                    EditorGUILayout.Space(SpaceAfterBoxedHeader);
            }

            if (expanded || stripped)
            {
                // begin dummy vertical group,
                // because MaterialEditor interrupts one in OnInspectorGUI
                if (materialEditor)
                {
                    EditorGUILayout.BeginVertical();
                }

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
    }
}
