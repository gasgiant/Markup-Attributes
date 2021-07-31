using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public class TogglableValueWrapper
    {
        private readonly SerializedProperty serializedProperty;

        private readonly MaterialPropertiesWrapper materialProperties;
        private readonly int materialPropertyIndex;
        private readonly Material targetMaterial;
        private readonly string shaderKeyword;

        private MaterialProperty materialProperty
        {
            get
            {
                if (materialProperties != null)
                    return materialProperties.value[materialPropertyIndex];
                return null;
            }
        }

        public SerializedProperty TargetSerializedProperty => serializedProperty;

        public static TogglableValueWrapper Create(SerializedProperty serializedProperty)
        {
            if (serializedProperty.propertyType == SerializedPropertyType.Boolean)
                return new TogglableValueWrapper(serializedProperty);
            return null;
        }

        public static TogglableValueWrapper Create(int index, MaterialPropertiesWrapper materialProperties)
        {
            if (materialProperties.value[index].type == MaterialProperty.PropType.Float)
                return new TogglableValueWrapper(index, materialProperties);
            return null;
        }

        public static TogglableValueWrapper Create(int index, MaterialPropertiesWrapper materialProperties, 
            Material material, string keyword)
        {
            if (material != null && keyword != null &&
                materialProperties.value[index].type == MaterialProperty.PropType.Float)
            {
                return new TogglableValueWrapper(index, materialProperties, material, keyword);
            }
            return null;
        }

        public bool HasMixedValue
        {
            get
            {
                if (serializedProperty != null && serializedProperty.hasMultipleDifferentValues)
                    return true;
                if (materialProperty != null && materialProperty.hasMixedValue)
                {
                    return true;
                }
                return false;
            }
        }

        private TogglableValueWrapper(SerializedProperty serializedProperty)
        {
            this.serializedProperty = serializedProperty;
        }

        private TogglableValueWrapper(int index, MaterialPropertiesWrapper materialProperties, 
            Material material = null, string shaderKeyword = null)
        {
            this.materialPropertyIndex = index;
            this.materialProperties = materialProperties;
            targetMaterial = material;
            this.shaderKeyword = shaderKeyword;
        }

        public bool GetValue()
        {
            if (serializedProperty != null)
                return serializedProperty.boolValue;

            if (materialProperty != null)
            {
                bool b = materialProperty.floatValue > 0;
                SetKeywordOnMaterial(b);
                return b;
            }

            return false;
        }

        public void SetValue(bool b, bool forceIfMixed)
        {
            if (serializedProperty != null)
            {
                if (!forceIfMixed && serializedProperty.hasMultipleDifferentValues)
                    return;
                serializedProperty.boolValue = b;
                return;
            }

            if (materialProperty != null)
            {
                if (!forceIfMixed && materialProperty.hasMixedValue)
                    return;
                materialProperty.floatValue = b ? 1 : 0;
                SetKeywordOnMaterial(b);
                return;
            }
        }

        private void SetKeywordOnMaterial(bool b)
        {
            if (targetMaterial != null && shaderKeyword != null)
            {
                if (b)
                    targetMaterial.EnableKeyword(shaderKeyword);
                else
                    targetMaterial.DisableKeyword(shaderKeyword);
            }
        }
    }
}
