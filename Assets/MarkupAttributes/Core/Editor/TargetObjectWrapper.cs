using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    internal class TargetObjectWrapper
    {
        public object Target => targetObject;

        private object targetObject;
        private readonly SerializedProperty serializedProperty;

        public TargetObjectWrapper(object targetObject, SerializedProperty serializedProperty = null)
        {
            this.targetObject = targetObject;
            if (serializedProperty != null && targetObject.GetType().IsValueType)
                this.serializedProperty = serializedProperty;
        }

        public void Update()
        {
            if (serializedProperty != null)
            {
                targetObject = MarkupEditorUtils.GetTargetObjectOfProperty(serializedProperty);
            }
        }
    }
}
