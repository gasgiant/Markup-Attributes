using UnityEditor;
using UnityEngine;

namespace MarkupAttributes
{
    public static class MarkupStyles
    {
        private static GUIStyle boldFoldout;
        public static GUIStyle BoldFoldout
        {
            get
            {
                if (boldFoldout == null)
                {
                    boldFoldout = new GUIStyle(EditorStyles.foldout);
                    boldFoldout.font = EditorStyles.boldFont;
                }
                return boldFoldout;
            }
        }


        public static GUIStyle GroupBox => EditorStyles.helpBox;

        private static GUIStyle frameBox;
        public static GUIStyle TabsBox
        {
            get
            {
                if (frameBox == null)
                {
                    frameBox = new GUIStyle("FrameBox");
                    frameBox.padding = EditorStyles.helpBox.padding;
                }
                return frameBox;
            }
        }

        public static GUIStyle HeaderBox(bool opened)
        {
            InitializeHeaderStyles();
            if (opened)
                return EditorGUIUtility.isProSkin ? headerBoxOpenedDark : headerBoxOpenedLight;
            else
                return EditorGUIUtility.isProSkin ? headerBoxClosedDark : headerBoxClosedLight;
        } 

        private static bool headerStylesInitialized;
        private static GUIStyle headerBoxOpenedDark;
        private static GUIStyle headerBoxOpenedLight;
        private static GUIStyle headerBoxClosedDark;
        private static GUIStyle headerBoxClosedLight;

        private static void InitializeHeaderStyles()
        {
            if (!headerStylesInitialized)
            {
                headerBoxOpenedDark = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderOpened_Dark"));
                headerBoxOpenedLight = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderOpened_Light"));
                headerBoxClosedDark = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderClosed_Dark"));
                headerBoxClosedLight = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderClosed_Light"));
                headerStylesInitialized = true;
            }
        }

        private static GUIStyle CreateBoxStyle(Texture2D texture)
        {
            var style = new GUIStyle(EditorStyles.helpBox);
            if (texture != null)
            {
                style.normal.background = texture;
                style.normal.scaledBackgrounds = new Texture2D[] { texture };
            }
            return style;
        }

        private static Texture2D GetTexture(string name)
        {
            string[] results = AssetDatabase.FindAssets(name);
            if (results != null && results.Length > 0)
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(results[0]));
            return null;
        }
    }
}
