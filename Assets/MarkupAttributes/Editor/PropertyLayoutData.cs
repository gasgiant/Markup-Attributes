using System.Collections.Generic;

namespace MarkupAttributes.Editor
{
    internal class PropertyLayoutData
    {
        public readonly InspectorLayoutGroup[] groups;
        public readonly EndGroupAttribute end;
        public readonly ConditionWrapper[] hideConditions;
        public readonly ConditionWrapper[] disableConditions;
        public bool isTopLevel = true;
        public bool alwaysHide = false;
        public bool includeChildren = true;
        public int numberOfScopesToClose;

        public bool IsVisible()
        {
            if (alwaysHide)
                return false;
            for (int i = 0; i < hideConditions.Length; i++)
            {
                if (hideConditions[i].GetValue())
                    return false;
            }
            return true;
        }

        public bool IsEnabled()
        {
            for (int i = 0; i < disableConditions.Length; i++)
            {
                if (disableConditions[i].GetValue())
                    return false;
            }
            return true;
        }

        public PropertyLayoutData(List<InspectorLayoutGroup> groups, 
            List<ConditionWrapper> hideConditions, List<ConditionWrapper> disableConditions,
            EndGroupAttribute end)
        {
            groups.Sort((g0, g1) => g0.Order().CompareTo(g1.Order()));

            this.groups = groups.ToArray();
            this.hideConditions = hideConditions.ToArray();
            this.disableConditions = disableConditions.ToArray();
            this.end = end;
        }
    }
}
