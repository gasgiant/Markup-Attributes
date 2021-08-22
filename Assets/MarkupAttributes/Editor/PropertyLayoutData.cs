using System.Collections.Generic;

namespace MarkupAttributes.Editor
{
    internal class PropertyLayoutData
    {
        public readonly InspectorLayoutGroup[] groups;
        public readonly EndGroupAttribute end;
        public bool topLevel = true;
        public bool hide = false;
        public bool includeChildren = true;
        public int numberOfScopesToClose;

        public PropertyLayoutData(List<InspectorLayoutGroup> groups, EndGroupAttribute end)
        {
            groups.Sort((g0, g1) => g0.Order().CompareTo(g1.Order()));

            this.groups = groups.ToArray();
            this.end = end;
        }
    }
}
