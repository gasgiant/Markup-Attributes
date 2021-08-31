using System;
using System.Collections.Generic;
using UnityEditor;

namespace MarkupAttributes.Editor
{
    public enum CallbackEvent
    {
        BeforeProperty,
        ReplaceProperty,
        AfterProperty
    }

    internal class CallbackManager
    {
        private readonly bool usedByShaderGUI;
        private readonly SerializedProperty[] properties;
        private readonly MaterialPropertiesWrapper materialProperties;
        private readonly MaterialEditor materialEditor;

        private readonly Dictionary<string, int> propertyIndices = new Dictionary<string, int>();
        private readonly Dictionary<(int, CallbackEvent), Action<SerializedProperty>> callbacks;
        private readonly Dictionary<(int, CallbackEvent), Action<MaterialEditor, MaterialProperty[], MaterialProperty>> shaderGuiCallbacks;

        public CallbackManager(SerializedProperty[] props)
        {
            properties = props;
            callbacks = new Dictionary<(int, CallbackEvent), Action<SerializedProperty>>();
            for (int i = 0; i < props.Length; i++)
            {
                if (!propertyIndices.ContainsKey(props[i].name))
                    propertyIndices.Add(props[i].name, i);
            }
        }

        public CallbackManager(MaterialEditor materialEditor, MaterialPropertiesWrapper materialProperties)
        {
            usedByShaderGUI = true;
            this.materialEditor = materialEditor;
            this.materialProperties = materialProperties;
            shaderGuiCallbacks = new Dictionary<(int, CallbackEvent), Action<MaterialEditor, MaterialProperty[], MaterialProperty>>();
            for (int i = 0; i < materialProperties.value.Length; i++)
            {
                if (!propertyIndices.ContainsKey(materialProperties.value[i].name))
                    propertyIndices.Add(materialProperties.value[i].name, i);
            }
        }

        public void AddCallback(SerializedProperty property, CallbackEvent callbackType, 
            Action<SerializedProperty> action)
        {
            if (propertyIndices.ContainsKey(property.name))
            {
                var key = (propertyIndices[property.name], callbackType);
                if (callbacks.ContainsKey(key))
                {
                    UnityEngine.Debug.LogError("Callback for property " + property.name +
                        " with type " + callbackType.ToString() + " already exists.");
                }
                else
                {
                    callbacks.Add(key, action);
                }
            }
        }

        public void AddCallback(MaterialProperty property, CallbackEvent callbackType,
            Action<MaterialEditor, MaterialProperty[], MaterialProperty> action)
        {

            if (propertyIndices.ContainsKey(property.name))
            {
                var key = (propertyIndices[property.name], callbackType);
                if (shaderGuiCallbacks.ContainsKey(key))
                {
                    UnityEngine.Debug.LogError("Callback for property " + property.name +
                        " with type " + callbackType.ToString() + " already exists.");
                }
                else
                {
                    shaderGuiCallbacks.Add(key, action);
                }
            }
        }

        public bool InvokeCallback(int index, CallbackEvent hookType)
        {
            var id = (index, hookType);
            if (!usedByShaderGUI)
            {
                if (callbacks.ContainsKey(id))
                {
                    callbacks[id].Invoke(properties[index]);
                    return true;
                }
            }
            else
            {
                if (shaderGuiCallbacks.ContainsKey(id))
                {
                    shaderGuiCallbacks[id].Invoke(materialEditor, 
                        materialProperties.value, materialProperties.value[index]);
                    return true;
                }
            }
            return false;
        }
    }
}
