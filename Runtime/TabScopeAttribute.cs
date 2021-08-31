using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TabScopeAttribute : LayoutGroupAttribute
    {
        public TabScopeAttribute(string path, string tabs, bool box = false)
        {
            Path = path;
            Type = LayoutGroupType.TabScope;
            Tabs = tabs.Split('|');
            if (box)
                BodyStyle = MarkupBodyStyle.Box;
        }
    }
}
