using System.Collections.Generic;
using UnityEditor;

namespace MarkupAttributes.Editor
{
    internal class SerializedDictionaryHandler
    {
        private readonly SerializedProperty array;
        private Dictionary<string, SerializedProperty> dictionary;

        public SerializedDictionaryHandler(SerializedProperty targetArray)
        {
            array = targetArray;
            dictionary = new Dictionary<string, SerializedProperty>();

            for (int i = 0; i < array.arraySize; i++)
            {
                var prop = array.GetArrayElementAtIndex(i);
                dictionary.Add(prop.FindPropertyRelative("key").stringValue,
                    prop.FindPropertyRelative("value"));
            }
        }

        public void DeleteAll()
        {
            array.arraySize = 0;
            array.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var prop = GetValueProp(key, false);
            return prop == null ? defaultValue : prop.boolValue;
        }
        public int GetInt(string key, int defaultValue = 0)
        {
            var prop = GetValueProp(key, false);
            return prop == null ? defaultValue : prop.intValue;
        }
        public void SetBool(string key, bool value)
        {
            var prop = GetValueProp(key, true);
            prop.boolValue = value;
            array.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        public void SetInt(string key, int value)
        {
            var prop = GetValueProp(key, true);
            prop.intValue = value;
            array.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private SerializedProperty GetValueProp(string key, bool createIfNotPresent)
        {
            if (dictionary.TryGetValue(key, out SerializedProperty prop))
                return prop;
            else
                return createIfNotPresent ? AddKey(key) : null;
        }

        private SerializedProperty AddKey(string key)
        {
            array.arraySize += 1;
            SerializedProperty prop = array.GetArrayElementAtIndex(array.arraySize - 1);
            prop.FindPropertyRelative("key").stringValue = key;
            SerializedProperty valueProp = prop.FindPropertyRelative("value");
            dictionary.Add(key, valueProp);
            return valueProp;
        }
    }
}
