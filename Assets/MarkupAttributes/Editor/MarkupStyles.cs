using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public static class MarkupStyles
    {
        public static GUIStyle OutlinedBox => EditorStyles.helpBox;
        public static GUIStyle Box => GUI.skin.box;

        public static GUIStyle CenteredLabel
        {
            get
            {
                if (centeredLabel == null)
                {
                    centeredLabel = new GUIStyle(EditorStyles.label);
                    centeredLabel.alignment = TextAnchor.MiddleCenter;
                }
                return centeredLabel;
            }
        }

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

        public static GUIStyle OutlinedHeaderBox(bool opened)
        {
            InitializeHeaderStyles();
            if (opened)
                return EditorGUIUtility.isProSkin ? headerBoxOpenedDark : headerBoxOpenedLight;
            else
                return EditorGUIUtility.isProSkin ? headerBoxClosedDark : headerBoxClosedLight;
        }

        public static GUIStyle TabOnlyOne = "Tab onlyOne";
        public static GUIStyle TabFirst = "Tab first";
        public static GUIStyle TabMiddle = "Tab middle";
        public static GUIStyle TabLast = "Tab last";
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

        private static GUIStyle centeredLabel;
        private static GUIStyle boldFoldout;
        private static GUIStyle frameBox;
        private static GUIStyle headerBoxOpenedDark;
        private static GUIStyle headerBoxOpenedLight;
        private static GUIStyle headerBoxClosedDark;
        private static GUIStyle headerBoxClosedLight;

        private static void InitializeHeaderStyles()
        {
            if (headerBoxOpenedDark == null)
            {
                headerBoxOpenedDark = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderOpened_Dark"));
                headerBoxOpenedLight = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderOpened_Light"));
                headerBoxClosedDark = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderClosed_Dark"));
                headerBoxClosedLight = CreateBoxStyle(GetTexture("MarkupAttributes_HeaderClosed_Light"));
            }
        }

        private static GUIStyle CreateBoxStyle(Texture2D texture)
        {
            var style = new GUIStyle(EditorStyles.helpBox);
            if (texture != null)
            {
                style.normal.background = texture;
                style.normal.scaledBackgrounds = new Texture2D[0];
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
