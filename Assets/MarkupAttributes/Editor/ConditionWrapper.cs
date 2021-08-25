using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public class ConditionWrapper
    {
        private bool alwaysTrue;
        private bool alwaysFalse;
        private readonly bool inverted;

        private readonly object targetObject;
        private readonly SerializedProperty parentProperty;
        private readonly bool isParentValueType;
        private readonly FieldInfo fieldInfo;
        private readonly PropertyInfo propertyInfo;
        private readonly MethodInfo methodInfo;

        private readonly int materialPropertyIndex;
        private readonly Material targetMaterial;
        private readonly string shaderKeyword;
        private readonly bool isShaderKeywordGlobal;
        private readonly MaterialPropertiesWrapper materialProperties;

        public static ConditionWrapper Create(string condition, bool inverted, object targetObject, SerializedProperty parentProperty = null)
        {
            if (targetObject != null)
            {
                Type type = targetObject.GetType();

                FieldInfo fieldInfo = type.GetField(condition, MarkupEditorUtils.DefaultBindingFlags);
                if (fieldInfo != null && fieldInfo.FieldType == typeof(bool))
                {
                    return new ConditionWrapper(inverted, targetObject, parentProperty, type.IsValueType, fieldInfo, null, null);
                }

                PropertyInfo propertyInfo = type.GetProperty(condition, MarkupEditorUtils.DefaultBindingFlags);
                if (propertyInfo != null && propertyInfo.PropertyType == typeof(bool))
                {
                    return new ConditionWrapper(inverted, targetObject, parentProperty, type.IsValueType, null, propertyInfo, null);
                }

                MethodInfo methodInfo = type.GetMethod(condition, MarkupEditorUtils.DefaultBindingFlags);
                if (methodInfo != null && methodInfo.ReturnType == typeof(bool)
                    && methodInfo.GetParameters().Length == 0)
                {
                    return new ConditionWrapper(inverted, targetObject, parentProperty, type.IsValueType, null, null, methodInfo);
                }
            }
            return null;
        }

        public static ConditionWrapper Create(string condition, bool inverted, 
            MaterialPropertiesWrapper materialProperties, Material targetMaterial)
        {
            var props = materialProperties.value;
            for (int i = 0; i < props.Length; i++)
            {
                var materialProperty = props[i];
                if ((materialProperty.type == MaterialProperty.PropType.Float
                                    || materialProperty.type == MaterialProperty.PropType.Range)
                                    && materialProperty.name == condition)
                {
                    return new ConditionWrapper(inverted, i, materialProperties,
                        null, null, false);
                }
            }

            string keyword = ShaderAttributesParser.GetKeyword(condition, out bool isGlobal);
            return new ConditionWrapper(inverted, -1, null, targetMaterial, keyword, isGlobal);
        }

        private ConditionWrapper(bool inverted, object targetObject, 
            SerializedProperty parentProperty, bool isParentValueType,
            FieldInfo fieldInfo, PropertyInfo propertyInfo, MethodInfo methodInfo)
        {
            this.inverted = inverted;
            this.targetObject = targetObject;
            this.parentProperty = parentProperty;
            this.isParentValueType = isParentValueType;
            this.fieldInfo = fieldInfo;
            this.propertyInfo = propertyInfo;
            this.methodInfo = methodInfo;
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

            object target = targetObject;
            if (parentProperty != null && isParentValueType)
                target = MarkupEditorUtils.GetTargetObjectOfProperty(parentProperty);

            if (target != null)
            {
                if (fieldInfo != null)
                    return (bool)fieldInfo.GetValue(target) ^ inverted;

                if (propertyInfo != null)
                    return (bool)propertyInfo.GetValue(target) ^ inverted;

                if (methodInfo != null)
                    return (bool)methodInfo.Invoke(target, null) ^ inverted;
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
    }
}
