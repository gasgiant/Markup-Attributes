using UnityEngine;
using UnityEditor;
using System;

namespace MarkupAttributes.Editor
{
    public enum CompactTextureMode { Default, UniformScaleOnly, ScaleOnly }

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
        private static bool drawSystemProperties;

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
                
                if (layoutController.ScopeVisible)
                {
                    callbackManager.InvokeCallback(i, CallbackEvent.BeforeProperty);
                    using (new EditorGUI.DisabledScope(!layoutController.ScopeEnabled))
                    {
                        if (!layoutController.Hide(i) && !callbackManager.InvokeCallback(i, CallbackEvent.ReplaceProperty))
                            DrawProperty(materialEditor, properties[i], allAttributes[i]);
                    }
                    callbackManager.InvokeCallback(i, CallbackEvent.AfterProperty);
                }
            }
            layoutController.Finish();

            if (drawSystemProperties)
            {
                EditorGUILayout.Space();
                materialEditor.RenderQueueField();
                materialEditor.DoubleSidedGIField();
            }
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
            drawSystemProperties = ShaderAttributesParser.GetDrawSystemPropertiesAttribute(allAttributes);
            
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

        private void DrawProperty(MaterialEditor editor, MaterialProperty prop, string[] attributes)
        {
            if (prop.flags.HasFlag(MaterialProperty.PropFlags.HideInInspector))
                return;

            if (prop.type == MaterialProperty.PropType.Texture)
            {
                var mode = ShaderAttributesParser.GetCompactTextureAttribute(attributes);
                if (mode.HasValue)
                    CompactTextureField(editor, prop, mode.Value);
                else
                    editor.ShaderProperty(prop, MakeLabel(prop));
            }
            else
                editor.ShaderProperty(prop, MakeLabel(prop));
        }

        private void CompactTextureField(MaterialEditor editor, 
            MaterialProperty prop, CompactTextureMode mode)
        {

            Rect fullRect = EditorGUILayout.GetControlRect(false);
            editor.TexturePropertyMiniThumbnail(fullRect, prop, prop.displayName, null);
            if (prop.flags.HasFlag(MaterialProperty.PropFlags.NoScaleOffset))
                return;

            Vector4 currentScaleOffset = prop.textureScaleAndOffset;
            float[] scale = new float[] { currentScaleOffset.x, currentScaleOffset.y };
            float[] offset = new float[] { currentScaleOffset.z, currentScaleOffset.w };
            
            Rect scaleRect = fullRect;
            scaleRect.x += EditorGUIUtility.labelWidth;
            scaleRect.width -= EditorGUIUtility.labelWidth;
            float labelWidth = EditorStyles.label.CalcSize(OffsetLabel).x;
            labelWidth += EditorGUIUtility.standardVerticalSpacing * 5;
            Rect scaleLabelRect = scaleRect;
            scaleLabelRect.width = labelWidth;
            Rect scaleFieldRect = scaleRect;
            scaleFieldRect.x += labelWidth;
            scaleFieldRect.width -= labelWidth;

            if (mode == CompactTextureMode.UniformScaleOnly)
            {
                float previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth;
                scale[0] = EditorGUI.FloatField(scaleRect, ScaleLabel, scale[0]);
                EditorGUIUtility.labelWidth = previousLabelWidth;
                scale[1] = scale[0];
            }
            else
            {
                EditorGUI.LabelField(scaleLabelRect, ScaleLabel);
                MultiFloatField(scaleFieldRect, scale);
            }

            if (mode == CompactTextureMode.Default)
            {
                EditorGUILayout.GetControlRect(false);
                Rect offsetLabelRect = scaleLabelRect;
                Rect offsetFieldRect = scaleFieldRect;
                offsetLabelRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                offsetFieldRect.y = offsetLabelRect.y;
                EditorGUI.LabelField(offsetLabelRect, OffsetLabel);
                MultiFloatField(offsetFieldRect, offset);
            }

            prop.textureScaleAndOffset = new Vector4(scale[0], scale[1], offset[0], offset[1]);
        }

        private void MultiFloatField(Rect rect, float[] floats)
        {
            GUIContent[] labels = { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
            EditorGUI.MultiFloatField(rect, labels, floats);
        }

        private static GUIContent TempLabel = new GUIContent();
        private static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
        {
            TempLabel.text = property.displayName;
            TempLabel.tooltip = tooltip;
            return TempLabel;
        }

        private static readonly GUIContent ScaleLabel = new GUIContent("Tiling");
        private static readonly GUIContent OffsetLabel = new GUIContent("Offset");
    }
}

