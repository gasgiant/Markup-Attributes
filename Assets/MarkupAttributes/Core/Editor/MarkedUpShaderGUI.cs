using UnityEngine;
using UnityEditor;
using System;

namespace MarkupAttributes.Editor
{
    [CanEditMultipleObjects()]
    public class MarkedUpShaderGUI : ShaderGUI
    {
        private static MaterialEditor currentEditor;
        private static MaterialPropertiesWrapper materialPropertiesWrapper;
        private static string[][] allAttributes;
        private static InspectorLayoutController layoutController;
        private static CallbackManager callbackManager;
        private static Shader shader;
        private static Material material;
        private static int systemPropertiesIndex;

        protected virtual void OnInitialize(MaterialEditor materialEditor, MaterialProperty[] properties) { }

        protected void AddCallback(MaterialProperty property, CallbackEvent type, 
            Action<MaterialEditor, MaterialProperty[], MaterialProperty> callback)
        {
            callbackManager.AddCallback(property, type, callback);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Initialize(materialEditor, properties);

            layoutController.Begin();
            for (int i = 0; i < properties.Length; i++)
            {
                layoutController.BeforeProperty(i);
                
                if (layoutController.ScopeVisible && layoutController.IsPropertyVisible(i))
                {
                    callbackManager.InvokeCallback(i, CallbackEvent.BeforeProperty);
                    using (new EditorGUI.DisabledScope(
                        !layoutController.ScopeEnabled || !layoutController.IsPropertyEnabled(i)))
                    {
                        if (!callbackManager.InvokeCallback(i, CallbackEvent.ReplaceProperty))
                            DrawProperty(materialEditor, properties[i]);
                    }
                    callbackManager.InvokeCallback(i, CallbackEvent.AfterProperty);

                    if (i == systemPropertiesIndex)
                    {
                        EditorGUILayout.Space();
                        DrawSystemProperties(materialEditor);
                    }

                }
            }
            layoutController.Finish();
        }

        private void Initialize(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (currentEditor == materialEditor && allAttributes != null
                && properties.Length == materialPropertiesWrapper.value.Length)
            {
                materialPropertiesWrapper.value = properties;
                return;
            }

            material = (Material)materialEditor.target;
            shader = material.shader;
            currentEditor = materialEditor;
            allAttributes = GetAttributes(shader, properties.Length);
            materialPropertiesWrapper = new MaterialPropertiesWrapper();
            materialPropertiesWrapper.value = properties;
            layoutController = new InspectorLayoutController(shader.name,
                ShaderAttributesParser.GetLayoutData(allAttributes, materialPropertiesWrapper, material));
            callbackManager = new CallbackManager(materialEditor, materialPropertiesWrapper);
            systemPropertiesIndex = ShaderAttributesParser.GetDrawSystemPropertiesAttribute(allAttributes);
            
            OnInitialize(materialEditor, properties);
        }

        private string[][] GetAttributes(Shader shader, int propsCount)
        {
            var output = new string[propsCount][];
            for (int i = 0; i < propsCount; i++)
            {
                output[i] = shader.GetPropertyAttributes(i);
            }
            return output;
        }

        private void DrawProperty(MaterialEditor editor, MaterialProperty prop)
        {
            if (prop.flags.HasFlag(MaterialProperty.PropFlags.HideInInspector))
                return;
            bool hierarchyMode = EditorGUIUtility.hierarchyMode;
            if (prop.type == MaterialProperty.PropType.Range)
                EditorGUIUtility.hierarchyMode = true;
            editor.ShaderProperty(prop, MakeLabel(prop));
            EditorGUIUtility.hierarchyMode = hierarchyMode;
        }

        private void DrawSystemProperties(MaterialEditor materialEditor)
        {
            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
            materialEditor.DoubleSidedGIField();
        }

        private static GUIContent TempLabel = new GUIContent();
        private static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
        {
            TempLabel.text = property.displayName;
            TempLabel.tooltip = tooltip;
            return TempLabel;
        }

        
    }
}

