using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace MarkupAttributes.Editor
{
    internal class InlineEditorData
    {
        public UnityEditor.Editor editor;
        public InlineEditorMode mode;
        public bool enabled = true;

        public InlineEditorData(UnityEditor.Editor editor, InlineEditorAttribute attribute)
        {
            this.editor = editor;
            mode = attribute.Mode;
        }
    }

    internal static class EditorLayoutDataBuilder
    {
        public static void BuildLayoutData(SerializedObject serializedObject,
            out SerializedProperty[] allProps,
            out SerializedProperty[] topLevelProps,
            out List<PropertyLayoutData> layoutData,
            out Dictionary<SerializedProperty, InlineEditorData> inlineEditors,
            out List<TargetObjectWrapper> targetsRequireUpdate)
        {
            var props = new List<SerializedProperty>();
            layoutData = new List<PropertyLayoutData>();
            inlineEditors = new Dictionary<SerializedProperty, InlineEditorData>();
            targetsRequireUpdate = new List<TargetObjectWrapper>();
            Type targetType = serializedObject.targetObject.GetType();

            topLevelProps = MarkupEditorUtils.GetSerializedObjectProperties(serializedObject);
            GetLayoutDataForSiblings(null, topLevelProps, targetType, 
                new TargetObjectWrapper(serializedObject.targetObject),
                props, layoutData, inlineEditors, targetsRequireUpdate);
            allProps = props.ToArray();
        }

        private static int GetLayoutDataForSiblings(InspectorLayoutGroup scopeGroup,
            SerializedProperty[] siblings, Type targetType, TargetObjectWrapper targetObjectWrapper,
            List<SerializedProperty> allProps, List<PropertyLayoutData> layoutData,
            Dictionary<SerializedProperty, InlineEditorData> inlineEditors, List<TargetObjectWrapper> targetObjectWrappers)
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
                    var groupAttribues = fieldInfo.GetCustomAttributes<LayoutGroupAttribute>(true).ToArray();

                    bool isPropertyHidden = false;
                    foreach (var groupAttribute in groupAttribues)
                    {
                        var group = CreateGroupFromAttribute(ref isPropertyHidden, 
                            groupAttribute, sibling, targetObjectWrapper);
                        groups.Add(group);
                    }

                    // conditionals 
                    var hideConditions = new List<ConditionWrapper>();
                    foreach (var attribute in fieldInfo.GetCustomAttributes<HideIfAttribute>())
                    {
                        hideConditions.Add(ConditionWrapper.Create(attribute.Condition, targetObjectWrapper));
                    }

                    var disableConditions = new List<ConditionWrapper>();
                    foreach (var attribute in fieldInfo.GetCustomAttributes<DisableIfAttribute>())
                    {
                        disableConditions.Add(ConditionWrapper.Create(attribute.Condition, targetObjectWrapper));
                    }

                    var end = fieldInfo.GetCustomAttribute<EndGroupAttribute>();
                    data = new PropertyLayoutData(groups, hideConditions, disableConditions, end);
                    data.alwaysHide = isPropertyHidden;
                    data.isTopLevel = scopeGroup == null;
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
                    var markedUp = fieldInfo.GetCustomAttribute<MarkedUpTypeAttribute>();
                    if (markedUp == null)
                        markedUp = fieldInfo.FieldType.GetCustomAttribute<MarkedUpTypeAttribute>(true);

                    if (markedUp != null)
                    {
                        var subTarget = MarkupEditorUtils.GetTargetObjectOfProperty(sibling);
                        var subTargetType = subTarget.GetType();
                        var subTargetWrapper = new TargetObjectWrapper(subTarget, sibling);
                        if (subTargetType.IsValueType)
                            targetObjectWrappers.Add(subTargetWrapper);
                        if (subTargetType != targetType)
                        {
                            var children = MarkupEditorUtils.GetChildrenOfProperty(sibling).ToArray();
                            if (children != null && children.Length > 0)
                            {
                                data.includeChildren = false;
                                data.alwaysHide |= !markedUp.ShowControl;
                                var subScopeGroup = InspectorLayoutGroup.CreateScopeGroup(
                                    "./" + sibling.name, sibling, subTargetType.FullName, 
                                    markedUp.ShowControl, markedUp.IndentChildren);
                                scopesToClose += GetLayoutDataForSiblings(
                                    subScopeGroup, children, subTargetType, subTargetWrapper, 
                                    allProps, layoutData, inlineEditors, targetObjectWrappers);
                                scopesToClose += 1;
                            }
                        }
                    }
                }
            }
            return scopesToClose;
        }

        private static InspectorLayoutGroup CreateGroupFromAttribute(ref bool isHidden,
            LayoutGroupAttribute attribute, SerializedProperty property, TargetObjectWrapper targetObjectWrapper)
        {
            ConditionWrapper conditionWrapper = null;
            if (attribute.HasCondition)
            {
                conditionWrapper = ConditionWrapper.Create(attribute.Condition, targetObjectWrapper);
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


