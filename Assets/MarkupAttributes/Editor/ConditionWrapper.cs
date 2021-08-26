using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    internal class ConditionWrapper
    {
        private readonly bool alwaysTrue;
        private readonly bool alwaysFalse;
        private readonly bool inverted;

        private readonly TargetObjectWrapper targetObjectWrapper;
        private readonly FieldInfo fieldInfo;
        private readonly PropertyInfo propertyInfo;
        private readonly MethodInfo methodInfo;
        private readonly int? enumValue;

        private readonly int materialPropertyIndex;
        private readonly Material targetMaterial;
        private readonly string shaderKeyword;
        private readonly bool isShaderKeywordGlobal;
        private readonly MaterialPropertiesWrapper materialProperties;

        public static ConditionWrapper Create(ConditionDescriptor descriptor, TargetObjectWrapper targetObjectWrapper)
        {
            if (targetObjectWrapper != null && targetObjectWrapper.Target != null)
            {
                object targetObject = targetObjectWrapper.Target;
                Type type = targetObject.GetType();

                FieldInfo fieldInfo = type.GetField(descriptor.condition, MarkupEditorUtils.DefaultBindingFlags);
                if (fieldInfo != null)
                {
                    if (fieldInfo.FieldType == typeof(bool) 
                        || IsEnumCondition(fieldInfo.FieldType, descriptor.enumValue))
                        return new ConditionWrapper(descriptor.isInverted, targetObjectWrapper, fieldInfo, null, null, descriptor.enumValue);
                }

                PropertyInfo propertyInfo = type.GetProperty(descriptor.condition, MarkupEditorUtils.DefaultBindingFlags);
                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType == typeof(bool) 
                        || IsEnumCondition(propertyInfo.PropertyType, descriptor.enumValue))
                        return new ConditionWrapper(descriptor.isInverted, targetObjectWrapper, null, propertyInfo, null, descriptor.enumValue);
                }

                MethodInfo methodInfo = type.GetMethod(descriptor.condition, MarkupEditorUtils.DefaultBindingFlags);
                if (methodInfo != null && methodInfo.GetParameters().Length == 0)
                {
                    if (methodInfo.ReturnType == typeof(bool)
                        || IsEnumCondition(methodInfo.ReturnType, descriptor.enumValue))
                        return new ConditionWrapper(descriptor.isInverted, targetObjectWrapper, null, null, methodInfo, descriptor.enumValue);
                }
            }
            return null;
        }

        public static ConditionWrapper Create(ConditionDescriptor descriptor, 
            MaterialPropertiesWrapper materialProperties, Material targetMaterial)
        {
            var props = materialProperties.value;
            for (int i = 0; i < props.Length; i++)
            {
                var materialProperty = props[i];
                if ((materialProperty.type == MaterialProperty.PropType.Float
                                    || materialProperty.type == MaterialProperty.PropType.Range)
                                    && materialProperty.name == descriptor.condition)
                {
                    return new ConditionWrapper(descriptor.isInverted, i, materialProperties,
                        null, null, false);
                }
            }

            string keyword = ShaderAttributesParser.GetKeyword(descriptor.condition, out bool isGlobal);
            return new ConditionWrapper(descriptor.isInverted, -1, null, targetMaterial, keyword, isGlobal);
        }

        private ConditionWrapper(bool inverted, TargetObjectWrapper targetObjectWrapper, 
            FieldInfo fieldInfo, PropertyInfo propertyInfo, MethodInfo methodInfo, int? enumValue)
        {
            this.inverted = inverted;
            this.targetObjectWrapper = targetObjectWrapper;
            this.fieldInfo = fieldInfo;
            this.propertyInfo = propertyInfo;
            this.methodInfo = methodInfo;
            this.enumValue = enumValue;
        }

        private ConditionWrapper(bool inverted, int materialPropertyIndex, 
            MaterialPropertiesWrapper materialPropertiesWrapper,
            Material targetMaterial, string shaderKeyword, bool isShaderKeywordGlobal)
        {
            this.inverted = inverted;
            this.materialPropertyIndex = materialPropertyIndex;
            this.materialProperties = materialPropertiesWrapper;
            this.targetMaterial = targetMaterial;
            this.shaderKeyword = shaderKeyword;
            this.isShaderKeywordGlobal = isShaderKeywordGlobal;
        }

        public ConditionWrapper(bool value)
        {
            if (value)
                alwaysTrue = true;
            else
                alwaysFalse = false;
        }

        public bool GetValue()
        {
            if (alwaysTrue) return true;
            if (alwaysFalse) return false;

            object target = null;
            if (targetObjectWrapper != null)
                target = targetObjectWrapper.Target;

            if (target != null)
            {
                if (fieldInfo != null)
                    return FromMemberValue(fieldInfo.GetValue(target));

                if (propertyInfo != null)
                    return FromMemberValue(propertyInfo.GetValue(target));

                if (methodInfo != null)
                    return FromMemberValue(methodInfo.Invoke(target, null));
            }

            if (materialProperties != null)
            {
                return (materialProperties.value[materialPropertyIndex].floatValue > 0) ^ inverted;
            }

            if (isShaderKeywordGlobal && shaderKeyword != null)
            {
                return Shader.IsKeywordEnabled(shaderKeyword) ^ inverted;
            }

            if (targetMaterial != null && shaderKeyword != null)
            {
                return targetMaterial.IsKeywordEnabled(shaderKeyword) ^ inverted;
            }

            return false;
        }

        private bool FromMemberValue(object value)
        {
            if (enumValue.HasValue)
                return  ((int)value == enumValue.Value) ^ inverted;
            else
                return (bool)value ^ inverted;
        }

        private static bool IsEnumCondition(Type type, int? enumValue) =>
            typeof(Enum).IsAssignableFrom(type) && enumValue.HasValue;
    }
}
