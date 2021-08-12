using System;

namespace MarkupAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TabScopeAttribute : LayoutGroupAttribute
    {
        public TabScopeAttribute(string path, string tabs, bool boxed = false)
        {
            Path = path;
            Type = LayoutGroupType.TabScope;
            Tabs = tabs.Split('|');
            if (boxed)
                BodyStyle = MarkupBodyStyle.OutlinedBox;
        }
    }
}
