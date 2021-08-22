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

        protected virtual void OnInitialize() { }
        protected virtual void OnCleanup() { }
        protected void AddCallback(SerializedProperty property, CallbackEvent type, Action<SerializedProperty> callback)
        {
            callbackManager.AddCallback(property, type, callback);
        }

        protected void OnEnable()
        {
            InitializeMarkedUpInspector();
        }

        protected void OnDisable()
        {
            CleanupMarkedUpInspector();
        }

        public override void OnInspectorGUI()
        {
            DrawMarkedUpInspector();
        }

        protected void InitializeMarkedUpInspector()
        {
            EditorLayoutDataBuilder.BuildLayoutData(serializedObject, 
                out allProps, out firstLevelProps, out layoutData, out inlineEditors);
            layoutController = new InspectorLayoutController(target.GetType().FullName,
                layoutData.ToArray());
            callbackManager = new CallbackManager(firstLevelProps);
            OnInitialize();
        }

        protected void CleanupMarkedUpInspector()
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
            int topLevelIndex = 0;

            layoutController.Begin();
            for (int i = 0; i < allProps.Length; i++)
            {
                if (allProps[i].name.Equals("m_Script"))
                {
                    if (MarkupGUI.DrawScriptProperty)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.PropertyField(allProps[i]);
                        }
                    }
                }
                else
                {
                    layoutController.BeforeProperty(i);
                    if (layoutController.ScopeVisible)
                    {
                        using (new EditorGUI.DisabledScope(!layoutController.ScopeEnabled))
                        {
                            DrawProperty(i, topLevelIndex);
                        }
                    }
                }

                if (layoutController.TopLevel(i))
                    topLevelIndex += 1;
            }
            layoutController.Finish();

            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        private void DrawProperty(int index, int topLevelIndex)
        {
            var prop = allProps[index];
            bool topLevel = layoutController.TopLevel(index);

            if (topLevel) callbackManager.InvokeCallback(topLevelIndex, CallbackEvent.BeforeProperty);
            if (!topLevel || !callbackManager.InvokeCallback(index, CallbackEvent.ReplaceProperty))
            {
                if (!layoutController.Hide(index))
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
            if (topLevel) callbackManager.InvokeCallback(topLevelIndex, CallbackEvent.AfterProperty);
        }

        private void CreateInlineEditors()
        {
            var props = new List<SerializedProperty>(inlineEditors.Keys);
            foreach (var prop in props)
            {
                var editor = inlineEditors[prop].editor;
                Material material = prop.objectReferenceValue as Material;
                if (material != null)
                {
                    CreateCachedEditor(material, typeof(HeaderlessMaterialEditor), ref editor);
                    inlineEditors[prop].enabled = AssetDatabase.GetAssetPath(material).StartsWith("Assets");
                }
                else
                    CreateCachedEditor(prop.objectReferenceValue, null, ref editor);
                inlineEditors[prop].editor = editor;
            }
        }
    }
}
