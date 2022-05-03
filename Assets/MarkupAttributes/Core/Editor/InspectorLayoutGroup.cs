using System.Linq;
using UnityEditor;

namespace MarkupAttributes.Editor
{
    internal class InspectorLayoutGroup
    {
        public readonly Params data;
        public readonly string[] pathArray;
        public readonly string name;
        public readonly LocalScopeData localScope;
        public bool isVisible;
        public bool isEnabled;
        public string cachedPrefsPrefix = null;
        public int? cachedLocalScopeStart = null;
        public string cachedActiveTab = null;
        public MarkupGUI.GroupHandle? guiHandle;

        public int Order()
        {
            if (data.Type == LayoutGroupType.LocalScope)
                return pathArray.Length - 1000;
            if (pathArray.Length > 0 && (pathArray[0] == "." || pathArray[0] == ".."))
                return pathArray.Length + 1000;
            return pathArray.Length;
        }

        public static InspectorLayoutGroup CreateScopeGroup(string path, 
            SerializedProperty property, string prefsPrefix,
            bool showControl, bool indent)
        {
            return new InspectorLayoutGroup(path, 
                new LocalScopeData(property, prefsPrefix, showControl, indent));
        }

        public InspectorLayoutGroup(LayoutGroupAttribute attribute, ConditionWrapper conditionWrapper = null, 
            TogglableValueWrapper togglableValueWrapper = null)
        {
            data = new Params(attribute, conditionWrapper, togglableValueWrapper);
            SetNameAndPathArray(attribute.Path, out pathArray, out name);
        }

        private InspectorLayoutGroup(string path, LocalScopeData scopeData)
        {
            data = new Params(path, LayoutGroupType.LocalScope);
            localScope = scopeData;
            SetNameAndPathArray(path, out pathArray, out name);
        }

        private void SetNameAndPathArray(string path, 
            out string[] pathArray, out string name)
        {
            if (path != null)
            {
                pathArray = path.Split('/');
                if (pathArray.Length > 0)
                    name = pathArray.Last();
                else
                    name = "";
            }
            else
            {
                pathArray = new string[0];
                name = "";
            }
        }

        public class Params
        {
            public string Path => path;
            public LayoutGroupType Type => type;
            public MarkupHeaderFlags HeaderStyle => headerStyle;
            public MarkupBodyStyle BodyStyle => bodyStyle;
            public float LabelWidth => labelWidth;
            public float Space => space;
            public string[] Tabs => tabs;

            public readonly ConditionWrapper conditionWrapper;
            public readonly TogglableValueWrapper togglableValueWrapper;
            private readonly string path;
            private readonly LayoutGroupType type;
            private readonly MarkupHeaderFlags headerStyle;
            private readonly MarkupBodyStyle bodyStyle;
            private readonly float labelWidth;
            private readonly float space;
            private readonly string[] tabs;

            public Params(LayoutGroupAttribute attribute, ConditionWrapper conditionWrapper,
                TogglableValueWrapper togglableValueWrapper)
            {
                path = attribute.Path;
                type = attribute.Type;
                headerStyle = attribute.HeaderFlags;
                bodyStyle = attribute.BodyStyle;
                labelWidth = attribute.LabelWidth;
                space = attribute.Space;
                tabs = attribute.Tabs;
                this.conditionWrapper = conditionWrapper;
                this.togglableValueWrapper = togglableValueWrapper;
            }

            public Params(string path, LayoutGroupType type)
            {
                this.path = path;
                this.type = type;
            }
        }

        public class LocalScopeData
        {
            public readonly bool showControl;
            public readonly bool indent;
            public readonly string prefsPrefixOverride;
            private readonly SerializedProperty serializedProperty;
            public bool IsExpanded => serializedProperty.isExpanded;

            public LocalScopeData(SerializedProperty property, string prefsPrefix, bool showControl, bool indent)
            {
                serializedProperty = property;
                prefsPrefixOverride = prefsPrefix;
                this.showControl = showControl;
                this.indent = indent;
            }
        }
    }
}
