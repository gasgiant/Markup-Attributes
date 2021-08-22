using System;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public static partial class MarkupGUI
    {
        internal const float SpaceAfterBoxedHeader = 2;
        internal const float SpaceBeforeHeader = 3;

        #region Utility

        internal static bool DrawScriptProperty { get; private set; } = true;
        public struct DrawScriptPropertyScope : IDisposable
        {
            private readonly bool cachedDrawScriptProperty;

            public DrawScriptPropertyScope(bool draw)
            {
                cachedDrawScriptProperty = DrawScriptProperty;
                DrawScriptProperty &= draw;
            }

            public void Dispose()
            {
                DrawScriptProperty = cachedDrawScriptProperty;
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

        public static LabelState CurrentLabelState() => LabelState.CreateSnapshot();

        public struct LabelState
        {
            private bool? hierarchyMode;
            private int? indentLevel;
            private float? labelWidth;

            public static LabelState CreateSnapshot()
            {
                LabelState state = new LabelState();
                state.hierarchyMode = EditorGUIUtility.hierarchyMode;
                state.indentLevel = EditorGUI.indentLevel;
                state.labelWidth = EditorGUIUtility.labelWidth;
                return state;
            }

            public void ResetToThis()
            {
                if (hierarchyMode.HasValue)
                    EditorGUIUtility.hierarchyMode = hierarchyMode.Value;
                if (indentLevel.HasValue)
                    EditorGUI.indentLevel = indentLevel.Value;
                if (labelWidth.HasValue)
                    EditorGUIUtility.labelWidth = labelWidth.Value;
            }
        }

        public static LabelState StartNonHierarchyScope() => StartNonHierarchyScope(0);

        public static LabelState StartNonHierarchyScope(float padding)
        {
            bool previous = EditorGUIUtility.hierarchyMode;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.hierarchyMode = false;
            EditorGUIUtility.labelWidth = labelWidth - padding;
            return CurrentLabelState();
        }

        #endregion Utility

        #region Styling
        public static void HorizontalLine(float height = 1)
        {
            float c = EditorGUIUtility.isProSkin ? 0.45f : 0.4f;
            HorizontalLine(new Color(c, c, c), height);
        }

        public static void HorizontalLine(Color color, float height)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color);
            GUILayout.Space(2);
        }

        internal static Rect BeginVertical(MarkupBodyStyle style, bool hasHeader, bool isExpanded)
        {
            Rect headerRect = Rect.zero;
            if (style == MarkupBodyStyle.SeparatorLine)
            {
                EditorGUILayout.BeginVertical();
                if (hasHeader)
                {
                    GUILayout.Space(SpaceBeforeHeader);
                    headerRect = EditorGUILayout.GetControlRect();
                    if (isExpanded)
                    {
                        HorizontalLine();
                    }
                }
                return headerRect;
            }

            if (style == MarkupBodyStyle.ContentBox)
            {
                // Inlined editors create unwanted padding, 
                // if there is an empty GUIStyle.none vertical scope
                // (which can happen, for example, in unexpanded foldouts).
                // I don't know if it's a bug or an intended behaviour.
                // Workaround - put header inside the scope if it's not expanded.
                if (!isExpanded)
                    EditorGUILayout.BeginVertical();
                if (hasHeader)
                {
                    GUILayout.Space(SpaceBeforeHeader);
                    headerRect = EditorGUILayout.GetControlRect();
                }
                if (isExpanded)
                    EditorGUILayout.BeginVertical(MarkupStyles.Box);
                return headerRect;
            }

            if (style == MarkupBodyStyle.FullBox)
            {
                EditorGUILayout.BeginVertical(MarkupStyles.OutlinedBox);
                if (hasHeader)
                {
                    headerRect = EditorGUILayout.GetControlRect();
                    Rect headerBoxRect = MarkupStyles.OutlinedBox.padding.Add(headerRect);
                    GUI.Box(headerBoxRect, GUIContent.none, MarkupStyles.OutlinedHeaderBox(isExpanded));
                    if (isExpanded)
                    {
                        EditorGUILayout.Space(SpaceAfterBoxedHeader);
                    }
                }
                return headerRect;
            }

            EditorGUILayout.BeginVertical();
            return headerRect;
        }
        
        #endregion Styling


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

            float xOffset = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
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
                StartNonHierarchyScope(MarkupStyles.OutlinedBox.padding.left);
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
