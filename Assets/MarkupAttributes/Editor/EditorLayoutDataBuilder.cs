using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace MarkupAttributes.Editor
{
    public class InlineEditorData
    {
        public UnityEditor.Editor editor;
        public bool stripped;
        public bool enabled = true;

        public InlineEditorData(UnityEditor.Editor editor, InlineEditorAttribute attribute)
        {
            this.editor = editor;
            stripped = attribute.stripped;
        }
    }

    public static class EditorLayoutDataBuilder
    {
        public static void BuildLayoutData(SerializedObject serializedObject,
            out SerializedProperty[] allProps,
            out SerializedProperty[] firstLevelProps,
            out List<PropertyLayoutData> layoutData,
            out Dictionary<SerializedProperty, InlineEditorData> inlineEditors)
        {
            var props = new List<SerializedProperty>();
            layoutData = new List<PropertyLayoutData>();
            inlineEditors = new Dictionary<SerializedProperty, InlineEditorData>();
            Type targetType = serializedObject.targetObject.GetType();

            firstLevelProps = MarkupEditorUtils.GetSerializedObjectProperties(serializedObject);
            GetLayoutDataForSiblings(null, firstLevelProps, targetType, serializedObject.targetObject,
                props, layoutData, inlineEditors);
            allProps = props.ToArray();
        }

        private static int GetLayoutDataForSiblings(InspectorLayoutGroup scopeGroup,
            SerializedProperty[] siblings, Type targetType, object targetObject,
            List<SerializedProperty> allProps, List<PropertyLayoutData> layoutData,
            Dictionary<SerializedProperty, InlineEditorData> inlineEditors)
        {
            int scopesToClose = 0;
            for (int i = 0; i < siblings.Length; i++)
            {
                var sibling = siblings[i];
                var groups = new List<InspectorLayoutGroup>();
                if (scopeGroup != null && i == 0)
                {
                    groups.Add(scopeGroup);
                }

                FieldInfo fieldInfo = targetType.GetField(sibling.name, MarkupEditorUtils.DefaultBindingFlags);

                PropertyLayoutData data = null;
                if (fieldInfo != null)
                {
                    // layout groups
                    var groupAttribues = fieldInfo.GetCustomAttributes<LayoutGroupAttribute>().ToArray();

                    bool isPropertyHidden = false;
                    foreach (var groupAttribute in groupAttribues)
                    {
                        var group = CreateGroupFromAttribute(ref isPropertyHidden, 
                            groupAttribute, sibling, targetObject);
                        groups.Add(group);
                    }
                    var end = fieldInfo.GetCustomAttribute<EndGroupAttribute>();
                    data = new PropertyLayoutData(groups, end);
                    data.hide = isPropertyHidden;
                    data.topLevel = scopeGroup == null;
                    data.numberOfScopesToClose = scopesToClose;
                    scopesToClose = 0;

                    // InlineEditors
                    var inline = fieldInfo.GetCustomAttribute<InlineEditorAttribute>();
                    if (inline != null)
                        inlineEditors.Add(sibling, new InlineEditorData(null, inline));
                }

                allProps.Add(sibling);
                layoutData.Add(data);

                // Nested properties
                if (sibling.propertyType == SerializedPropertyType.Generic
                    && fieldInfo != null)
                {
                    var markedUp = fieldInfo.GetCustomAttribute<MarkedUpFieldAttribute>();
                    if (markedUp != null)
                    {
                        var subTarget = MarkupEditorUtils.GetTargetObjectOfProperty(sibling);
                        var subTargetType = subTarget.GetType();
                        if (subTargetType != targetType)
                        {
                            var children = MarkupEditorUtils.GetChildrenOfProperty(sibling).ToArray();
                            if (children != null && children.Length > 0)
                            {
                                data.includeChildren = false;
                                data.hide |= !markedUp.showControl;
                                var subScopeGroup = InspectorLayoutGroup.CreateScopeGroup(
                                    "./" + sibling.name, sibling, subTargetType.FullName, 
                                    markedUp.showControl, markedUp.indentChildren);
                                scopesToClose += GetLayoutDataForSiblings(
                                    subScopeGroup, children, subTargetType,
                                    subTarget, allProps, layoutData, inlineEditors);
                                scopesToClose += 1;
                            }
                        }
                    }
                }
            }
            return scopesToClose;
        }

        private static InspectorLayoutGroup CreateGroupFromAttribute(ref bool isHidden,
            LayoutGroupAttribute attribute, SerializedProperty property, object targetObject)
        {
            ConditionWrapper conditionWrapper = null;
            if (attribute.NeedsCondition)
            {
                conditionWrapper = ConditionWrapper.Create(
                    attribute.Condition, attribute.ConditionInverted, targetObject);
                if (conditionWrapper == null)
                    return null;
            }

            TogglableValueWrapper togglableValueWrapper = null;
            if (attribute.Toggle)
            {
                togglableValueWrapper = TogglableValueWrapper.Create(property);
                if (togglableValueWrapper == null)
                    return null;
                isHidden = true;
            }

            return new InspectorLayoutGroup(attribute, conditionWrapper, togglableValueWrapper);
        }
    }
}


