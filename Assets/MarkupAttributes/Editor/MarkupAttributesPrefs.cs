using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    internal class MarkupAttributesPrefs : ScriptableObject
    {
        private static MarkupAttributesPrefs instance;
        private static MarkupAttributesPrefs Instance
        {
            get
            {
                if (instance == null)
                {
                    var guids = AssetDatabase.FindAssets(
                        string.Format("t:{0}", typeof(MarkupAttributesPrefs)));

                    MarkupAttributesPrefs asset = null;
                    for (int i = 0; i < guids.Length; i++)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                        asset = AssetDatabase.LoadAssetAtPath<MarkupAttributesPrefs>(path);
                        if (asset != null) break;
                    }
                    if (asset == null)
                    {
                        asset = CreateInstance<MarkupAttributesPrefs>();
                        if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                            AssetDatabase.CreateFolder("Assets", "Editor");
                        AssetDatabase.CreateAsset(asset, "Assets/Editor/MarkupAttributesPersistentData.asset");
                        AssetDatabase.SaveAssets();
                    }

                    instance = asset;
                    SerializedObject so = new SerializedObject(instance);
                    instance.boolsHandler =
                        new SerializedDictionaryHandler(so.FindProperty("serializedBoolEntries"));
                    instance.intsHandler =
                        new SerializedDictionaryHandler(so.FindProperty("serializedIntEntries"));
                }
                return instance;
            }
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return Instance.boolsHandler.GetBool(key, defaultValue);
        }
        public static void SetBool(string key, bool value)
        {
            Instance.boolsHandler.SetBool(key, value);
        }
        public static int GetInt(string key, int defaultValue = 0)
        {
            return Instance.boolsHandler.GetInt(key, defaultValue);
        }
        public static void SetInt(string key, int value)
        {
            Instance.boolsHandler.SetInt(key, value);
        }

        public static void DeleteAll()
        {
            Instance.boolsHandler.DeleteAll();
            Instance.intsHandler.DeleteAll();
        }

        private SerializedDictionaryHandler boolsHandler;
        private SerializedDictionaryHandler intsHandler;

        [NonReorderable]
        [SerializeField] private BoolEntry[] serializedBoolEntries;
        [NonReorderable]
        [SerializeField] private IntEntry[] serializedIntEntries;

        [System.Serializable]
        private class BoolEntry
        {
            public string key;
            public bool value;
        }

        [System.Serializable]
        private class IntEntry
        {
            public string key;
            public int value;
        }
    }
}


