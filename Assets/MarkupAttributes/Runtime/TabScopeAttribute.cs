using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TabScopeAttribute : LayoutGroupAttribute
    {
        public TabScopeAttribute(string path, string tabs, bool boxed = false)
        {
            Path = path;
            Type = GroupType.TabScope;
            Tabs = tabs.Split('|');
            Style = GroupStyle.None;
            if (boxed)
                Style |= GroupStyle.Box;
        }
    }
}
