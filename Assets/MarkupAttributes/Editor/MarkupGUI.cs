using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public static partial class MarkupGUI
    {
        internal const float SpaceAfterBoxedHeader = 5;
        internal const float SpaceBeforeHeader = 3;

        #region Utility

        private static GUIContent tempContent = new GUIContent();
        private static GUIContent GetContent(string label = null, string tooltip = null)
        {
            if (label == null)
                return GUIContent.none;
            tempContent.text = label;
            tempContent.tooltip = tooltip;
            return tempContent;
        }

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

        private struct LabelState
        {
            private bool? hierarchyMode;
            private int? indentLevel;
            private float? labelWidth;

            public static LabelState Current()
            {
                LabelState state = new LabelState();
                state.hierarchyMode = EditorGUIUtility.hierarchyMode;
                state.indentLevel = EditorGUI.indentLevel;
                state.labelWidth = EditorGUIUtility.labelWidth;
                return state;
            }

            public void Restore()
            {
                if (hierarchyMode.HasValue)
                    EditorGUIUtility.hierarchyMode = hierarchyMode.Value;
                if (indentLevel.HasValue)
                    EditorGUI.indentLevel = indentLevel.Value;
                if (labelWidth.HasValue)
                    EditorGUIUtility.labelWidth = labelWidth.Value;
            }
        }

        internal static void StartNonHierarchyScope(float padding)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.hierarchyMode = false;
            EditorGUIUtility.labelWidth = labelWidth - padding;
        }

        public struct GroupHandle
        {
            private readonly LabelState labelState;
            private readonly bool isVertical;
            private readonly bool isHorizontal;

            public GroupHandle(bool isVertical, bool isHorizontal)
            {
                labelState = LabelState.Current();
                this.isVertical = isVertical;
                this.isHorizontal = isHorizontal;
            }

            public void End()
            {
                labelState.Restore();
                if (isVertical)
                    EditorGUILayout.EndVertical();
                if (isHorizontal)
                    EditorGUILayout.EndHorizontal();
            }
        }

        public class GroupsStack
        {
            private readonly Stack<GroupHandle> groups = new Stack<GroupHandle>();
            public void PushGroup(GroupHandle group) => groups.Push(group);

            public void EndGroup()
            {
                if (groups.Count < 1)
                {
                    Debug.LogWarning("No MarkupGUI groups to end.");
                }
                groups.Pop().End();
            }

            public void Clear()
            {
                groups.Clear();
            }

            public static GroupsStack operator +(GroupsStack stateStack, GroupHandle group)
            {
                stateStack.PushGroup(group);
                return stateStack;
            }

        }

        #endregion Utility

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
        public static Rect BeginVertical(MarkupBodyStyle style, bool hasHeader, bool isExpanded)
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

            if (style == MarkupBodyStyle.Box)
            {
                EditorGUILayout.BeginVertical(MarkupStyles.OutlinedBox);
                if (hasHeader)
                {
                    headerRect = EditorGUILayout.GetControlRect();
                    Rect headerBoxRect = MarkupStyles.OutlinedBox.padding.Add(headerRect);
                    GUI.Box(headerBoxRect, GUIContent.none, MarkupStyles.OutlinedHeaderBox(isExpanded));
                    if (isExpanded)
                    {
                        GUILayout.Space(SpaceAfterBoxedHeader);
                    }
                }
                return headerRect;
            }

            EditorGUILayout.BeginVertical();
            return headerRect;
        }


        internal static GroupHandle BeginGenericVerticalGroup(
            ref bool isExpanded, ref bool isEnabled,
            MarkupHeaderStyle headerStyle, MarkupBodyStyle bodyStyle,
            string label, TogglableValueWrapper togglableValue)
        {
            return BeginGenericVerticalGroup(ref isExpanded, ref isEnabled,
                headerStyle, bodyStyle, GetContent(label), togglableValue);
        }

        internal static GroupHandle BeginGenericVerticalGroup(
            ref bool isExpanded, ref bool isEnabled,
            MarkupHeaderStyle headerStyle, MarkupBodyStyle bodyStyle, 
            GUIContent label, TogglableValueWrapper togglableValue)
        {
            var handle = new GroupHandle(true, false);
            bool hasHeader = headerStyle != MarkupHeaderStyle.None;
            bool isFoldable = headerStyle == MarkupHeaderStyle.Foldable;
            isExpanded |= !isFoldable;

            Rect headerRect = BeginVertical(bodyStyle, hasHeader, isExpanded);

            if (bodyStyle == MarkupBodyStyle.Box)
            {
                StartNonHierarchyScope(MarkupStyles.OutlinedBox.padding.left);
            }

            if (hasHeader)
            {
                if (togglableValue != null)
                {
                    if (!isFoldable)
                        isExpanded = togglableValue.GetValue();
                    bool value = Toggle(
                        headerRect, togglableValue,
                        label, ref isExpanded, isFoldable);
                    if (isFoldable)
                    {
                        isEnabled &= value;
                    }
                    else
                        isExpanded &= value;
                }
                else
                {
                    if (isFoldable)
                    {
                        isExpanded = EditorGUI.Foldout(headerRect, isExpanded, label, true, MarkupStyles.BoldFoldout);
                    }
                    else
                    {
                        using (new EditorGUI.DisabledScope(!isEnabled))
                        {
                            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);
                        }
                    }
                }
            }

            if (bodyStyle == MarkupBodyStyle.ContentBox)
            {
                StartNonHierarchyScope(MarkupStyles.Box.padding.left);
            }

            return handle;
        }

        public static GroupHandle BeginBoxGroup(string label = null) => BeginBoxGroup(GetContent(label));

        public static GroupHandle BeginBoxGroup(GUIContent label)
        {
            bool isExpanded = true;
            bool isEnabled = true;
            return BeginGenericVerticalGroup(ref isExpanded, ref isEnabled,
                label != GUIContent.none ? MarkupHeaderStyle.Label : MarkupHeaderStyle.None,
                MarkupBodyStyle.Box, label, null);
        }

        public static GroupHandle BeginTitleGroup(string label, bool contentBox = false) 
            => BeginTitleGroup(GetContent(label), contentBox);

        public static GroupHandle BeginTitleGroup(GUIContent label, bool contentBox = false)
        {
            bool isExpanded = true;
            bool isEnabled = true;
            return BeginGenericVerticalGroup(ref isExpanded, ref isEnabled,
                MarkupHeaderStyle.Label, contentBox ? 
                MarkupBodyStyle.ContentBox : MarkupBodyStyle.SeparatorLine, label, null);
        }

        public static GroupHandle BeginFoldoutGroup(ref bool isExpanded, string label, bool box = true)
            => BeginFoldoutGroup(ref isExpanded, GetContent(label), box);

        public static GroupHandle BeginFoldoutGroup(ref bool isExpanded, GUIContent label, bool box = true)
        {
            bool isEnabled = true;
            return BeginGenericVerticalGroup(ref isExpanded, ref isEnabled,
                MarkupHeaderStyle.Foldable, box ?
                MarkupBodyStyle.Box : MarkupBodyStyle.ContentBox, label, null);
        }


        public static GroupHandle BeginTabsGroup(ref int selected, string[] tabs, bool box = false)
        {
            var handle = new GroupHandle(true, false);
            if (box)
            {
                EditorGUILayout.BeginVertical(MarkupStyles.TabsBox);
                StartNonHierarchyScope(MarkupStyles.TabsBox.padding.left);

                Rect position = EditorGUILayout.GetControlRect(false);
                var padding = MarkupStyles.OutlinedBox.padding;
                position.x -= padding.left;
                position.width += padding.right + padding.left - 1;
                position.y -= padding.top;
                position.height += padding.top;

                for (int i = 0; i < tabs.Length; i++)
                {
                    Rect r = GetTabRect(position, i, tabs.Length, out GUIStyle style);
                    if (EditorGUI.Toggle(r, i == selected, style))
                        selected = i;
                    EditorGUI.LabelField(r, tabs[i], MarkupStyles.CenteredLabel);
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(GUIStyle.none);
                selected = GUILayout.Toolbar(selected, tabs);
            }
            GUILayout.Space(SpaceAfterBoxedHeader);

            return handle;
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

        internal static bool Toggle(Rect position,
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

        public static void DrawEditorInline(SerializedProperty property,
            UnityEditor.Editor editor, InlineEditorMode mode, bool enabled = true)
        {
            var labelState = LabelState.Current();
            
            bool expanded = mode == InlineEditorMode.Stripped || property.isExpanded;
            expanded &= editor != null;
            expanded &= !property.hasMultipleDifferentValues;
            MaterialEditor materialEditor = editor as MaterialEditor;

            if (mode == InlineEditorMode.Box)
            {
                Rect headerRect = BeginVertical(MarkupBodyStyle.Box, true, property.isExpanded);
                StartNonHierarchyScope(MarkupStyles.OutlinedBox.padding.left);
                property.isExpanded = FoldoutWithObjectField(headerRect, property);
            }

            if (mode == InlineEditorMode.ContentBox)
            {
                Rect headerRect = BeginVertical(MarkupBodyStyle.ContentBox, true, property.isExpanded);
                property.isExpanded = FoldoutWithObjectField(headerRect, property);
                StartNonHierarchyScope(MarkupStyles.Box.padding.left);
            }

            if (mode == InlineEditorMode.Stripped)
            {
                if (editor == null)
                    FoldoutWithObjectField(EditorGUILayout.GetControlRect(), property);
            }

            if (expanded)
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

            if (mode != InlineEditorMode.Stripped)
            {
                EditorGUILayout.EndVertical();
                labelState.Restore();
            }
        }
    }
}
