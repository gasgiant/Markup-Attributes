using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    [CanEditMultipleObjects]
    public class MarkedUpEditor : UnityEditor.Editor
    {
        private SerializedProperty[] allProps;
        private SerializedProperty[] firstLevelProps;
        private List<PropertyLayoutData> layoutData;

        private InspectorLayoutController layoutController;
        private CallbackManager callbackManager;
        private Dictionary<SerializedProperty, InlineEditorData> inlineEditors;
        private List<TargetObjectWrapper> targetsRequireUpdate;

        protected virtual void OnInitialize() { }
        protected virtual void OnCleanup() { }
        protected void AddCallback(SerializedProperty property, CallbackEvent type, Action<SerializedProperty> callback)
        {
            callbackManager.AddCallback(property, type, callback);
        }

        protected void OnEnable()
        {
            InitializeMarkedUpEditor();
        }

        protected void OnDisable()
        {
            CleanupMarkedUpEditor();
        }

        public override void OnInspectorGUI()
        {
            DrawMarkedUpInspector();
        }

        protected void InitializeMarkedUpEditor()
        {
            EditorLayoutDataBuilder.BuildLayoutData(serializedObject, out allProps, 
                out firstLevelProps, out layoutData, out inlineEditors, out targetsRequireUpdate);
            layoutController = new InspectorLayoutController(target.GetType().FullName,
                layoutData.ToArray());
            callbackManager = new CallbackManager(firstLevelProps);
            OnInitialize();
        }

        protected void CleanupMarkedUpEditor()
        {
            OnCleanup();
            foreach (var item in inlineEditors)
            {
                DestroyImmediate(item.Value.editor);
            }
        }

        protected bool DrawMarkedUpInspector()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            CreateInlineEditors();
            UpdateTargets();
            int topLevelIndex = 1;
            layoutController.Begin();

            if (MarkupGUI.DrawScriptProperty)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(allProps[0]);
                }
            }

            for (int i = 1; i < allProps.Length; i++)
            {
                layoutController.BeforeProperty(i);
                if (layoutController.ScopeVisible)
                {
                    using (new EditorGUI.DisabledScope(!layoutController.ScopeEnabled))
                    {
                        DrawProperty(i, topLevelIndex);
                    }
                }

                if (layoutController.IsTopLevel(i))
                    topLevelIndex += 1;
            }
            layoutController.Finish();

            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        private void DrawProperty(int index, int topLevelIndex)
        {
            var prop = allProps[index];
            bool topLevel = layoutController.IsTopLevel(index);

            if (topLevel) callbackManager.InvokeCallback(topLevelIndex, CallbackEvent.BeforeProperty);
            

            using (new EditorGUI.DisabledScope(!layoutController.IsPropertyEnabled(index)))
            {
                if (layoutController.IsPropertyVisible(index))
                {
                    if (!topLevel || !callbackManager.InvokeCallback(index, CallbackEvent.ReplaceProperty))
                    {
                        if (inlineEditors.ContainsKey(prop))
                        {
                            InlineEditorData data = inlineEditors[prop];
                            MarkupGUI.DrawEditorInline(prop, data.editor, data.mode, data.enabled);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(prop, layoutController.IncludeChildren(index));
                        }
                    }
                }
            }
            if (topLevel) callbackManager.InvokeCallback(topLevelIndex, CallbackEvent.AfterProperty);
        }

        private void CreateInlineEditors()
        {
            var props = new List<SerializedProperty>(inlineEditors.Keys);
            foreach (var prop in props)
            {
                var editor = inlineEditors[prop].editor;

                if (prop.objectReferenceValue != serializedObject.targetObject)
                {
                    Material material = prop.objectReferenceValue as Material;
                    if (material != null)
                    {
                        CreateCachedEditor(material, typeof(HeaderlessMaterialEditor), ref editor);
                        inlineEditors[prop].enabled = AssetDatabase.GetAssetPath(material).StartsWith("Assets");
                    }
                    else
                        CreateCachedEditor(prop.objectReferenceValue, null, ref editor);
                }
                else
                {
                    editor = null;
                    prop.objectReferenceValue = null;
                    Debug.LogError("Self reference in the InlinedEditor property is not allowed.");
                }

                
                inlineEditors[prop].editor = editor;
            }
        }

        private void UpdateTargets()
        {
            foreach (var wrapper in targetsRequireUpdate)
            {
                wrapper.Update();
            }
        }
    }
}
