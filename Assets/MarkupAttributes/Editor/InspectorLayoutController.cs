using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public class InspectorLayoutController
    {
        public bool IncludeChildren(int index) => layoutData[index] == null || layoutData[index].includeChildren;
        public bool Hide(int index) => layoutData[index] == null || layoutData[index].hide;
        public bool TopLevel(int index) => layoutData[index] == null || layoutData[index].topLevel;
        public bool ScopeEnabled => groupsStack.Count == 0 || groupsStack.Peek().isEnabled;
        public bool ScopeVisible => groupsStack.Count == 0 || groupsStack.Peek().isVisible;
        
        private readonly string defaultPrefsPrefix;
        private readonly PropertyLayoutData[] layoutData;
        private readonly Stack<InspectorLayoutGroup> groupsStack = new Stack<InspectorLayoutGroup>();
        private List<string> currentPath = new List<string>();
        private string prefsPrefix = null;
        private int localScopeStart = -1;
        private string activeTabName;

        public InspectorLayoutController(string prefsPrefix, PropertyLayoutData[] layoutData)
        {
            defaultPrefsPrefix = prefsPrefix;
            this.layoutData = layoutData;
        }

        public void Begin()
        {
            groupsStack.Clear();
            currentPath.Clear();
            prefsPrefix = defaultPrefsPrefix;
            localScopeStart = -1;
            activeTabName = null;
        }

        public void Finish()
        {
            EndAll();
        }

        public void BeforeProperty(int index)
        {
            if (layoutData[index].end != null)
            {
                EndGroupsUntill(layoutData[index].end.name);
            }

            for (int i = 0; i < layoutData[index].numberOfScopesToClose; i++)
            {
                EndLocalScope();
            }

            if (layoutData[index].groups == null)
                return;

            foreach (var group in layoutData[index].groups)
            {
                SetScope(group.pathArray);
                bool isVisible = ScopeVisible;
                bool isEnabled = ScopeEnabled;

                if (group.data.Type == GroupType.LocalScope)
                {
                    group.cachedLocalScopeStart = localScopeStart;
                    localScopeStart = groupsStack.Count;
                }

                if (ScopeVisible)
                {
                    if (group.data.Type == GroupType.LocalScope)
                    {
                        if (group.localScope.indent)
                        {
                            group.cachedIndent = EditorGUI.indentLevel;
                            EditorGUI.indentLevel += 1;
                        }
                        isVisible &= !group.localScope.showControl || group.localScope.IsExpanded;
                        group.cachedPrefsPrefix = prefsPrefix;
                        prefsPrefix = group.localScope.prefsPrefixOverride;
                    }

                    if (group.data.Type == GroupType.DisableIf)
                    {
                        isEnabled &= !group.data.conditionWrapper.GetValue();
                    }

                    if (group.data.Type == GroupType.HideIf)
                    {
                        isVisible &= !group.data.conditionWrapper.GetValue();
                    }

                    if (group.data.Type == GroupType.Tab)
                    {
                        isVisible &= (activeTabName == null || activeTabName == group.name);
                    }

                    bool isBoxed = group.data.Style.HasFlag(GroupStyle.Box);
                    GUIStyle style = GUIStyle.none;
                    if (isBoxed)
                    {
                        style = MarkupStyles.GroupBox;
                        group.cachedHierarchyMode = EditorGUIUtility.hierarchyMode;
                        float labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.hierarchyMode = false;
                        EditorGUIUtility.labelWidth = labelWidth;
                    }

                    if (group.data.Type == GroupType.TabScope)
                    {
                        if (isBoxed)
                            style = MarkupStyles.TabsBox;
                        EditorGUILayout.BeginVertical(style);
                        string prefsName = GetPrefsName();
                        int activeTab;
                        activeTab = MarkupGUI.TabsControl(MarkupAttributesPrefs.GetInt(prefsName), group.data.Tabs, isBoxed);
                        MarkupAttributesPrefs.SetInt(prefsName, activeTab);
                        group.cachedActiveTab = activeTabName;
                        activeTabName = group.data.Tabs[activeTab];
                    }

                    

                    bool hasLabel = group.data.Style.HasFlag(GroupStyle.Label);
                    bool isFoldable = group.data.Style.HasFlag(GroupStyle.Foldable);

                    if (group.data.Type == GroupType.Vertical)
                        EditorGUILayout.BeginVertical(style);
                    if (group.data.Type == GroupType.Horizontal)
                    {
                        group.cachedLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = group.data.LabelWidth;
                        EditorGUILayout.BeginHorizontal(style);
                    }
                    
                    if (hasLabel)
                    {
                        string prefsName = GetPrefsName();
                        
                        bool isExpanded = !isFoldable || MarkupAttributesPrefs.GetBool(prefsName);
                        Rect headerRect;

                        if (group.data.Toggle)
                        {
                            if (!isFoldable)
                                isExpanded = group.data.togglableValueWrapper.GetValue();
                            headerRect = MarkupGUI.HeaderBase(group.data.Style, isExpanded);
                            bool value = MarkupGUI.ToggleGroupHeader(
                                headerRect, group.data.togglableValueWrapper, 
                                new GUIContent(group.name), ref isExpanded, isFoldable);
                            if (isFoldable)
                            {
                                MarkupAttributesPrefs.SetBool(prefsName, isExpanded);
                                isEnabled &= value;
                            }
                            else
                                isVisible &= value;
                        }
                        else
                        {
                            if (isFoldable)
                            {
                                isExpanded = MarkupAttributesPrefs.GetBool(prefsName);
                                headerRect = MarkupGUI.HeaderBase(group.data.Style, isExpanded);
                                isExpanded = EditorGUI.Foldout(headerRect, isExpanded, group.name, true, MarkupStyles.BoldFoldout);
                                MarkupAttributesPrefs.SetBool(prefsName, isExpanded);
                            }
                            else
                            {
                                using (new EditorGUI.DisabledScope(!ScopeEnabled))
                                {
                                    headerRect = MarkupGUI.HeaderBase(group.data.Style, true);
                                    EditorGUI.LabelField(headerRect, group.name, EditorStyles.boldLabel);
                                }
                            }
                        }

                        isVisible &= isExpanded;
                    }
                }

                group.isVisible = isVisible;
                group.isEnabled = isEnabled;
                groupsStack.Push(group);
            }
        }
        
        private string GetPrefsName()
        {
            if (prefsPrefix != null)
            {
                return prefsPrefix + "/" + string.Join("/", currentPath.ToArray(),
                    localScopeStart + 1, currentPath.Count - localScopeStart - 1);
            }
            else
            {
                return defaultPrefsPrefix + "/" + string.Join("/", currentPath);
            }
        }

        private void SetScope(string[] path)
        {
            if (path == null || path.Length < 1)
            {
                EndAll();
                currentPath.Add("");
                return;
            }

            if (path.Length > 1 && (path[0] == "." || path[0] == ".."))
            {
                if (currentPath.Count > 0 && path[0] == "..")
                {
                    EndGroup();
                }
                currentPath.Add(path.Last());
                return;
            }

            var newPath = new List<string>();
            int i = 0;
            int j = 0;
            while (j < path.Length && i < currentPath.Count)
            {
                if (i <= localScopeStart)
                {
                    newPath.Add(currentPath[i]);
                }
                else
                {
                    if (path[j] != currentPath[i])
                    {
                        break;
                    }
                    newPath.Add(path[j]);
                    j++;
                }
                i++;
            }

            if (j < path.Length)
                newPath.Add(path.Last());

            int groupsToRemove = currentPath.Count - i;
            for (int k = 0; k < groupsToRemove; k++)
            {
                EndGroup();
            }

            currentPath = newPath;
        }

        private void EndGroupsUntill(string name)
        {
            int limit = name != null ? -1 :
                Mathf.Max(-1, currentPath.Count - 2);
            limit = Mathf.Max(limit, localScopeStart);
            int index = currentPath.Count - 1;
            while (index > limit)
            {
                if (currentPath[index] != name)
                {
                    EndGroup();
                    index -= 1;
                }
                else
                {
                    EndGroup();
                    break;
                }     
            }
        }

        private void EndLocalScope()
        {
            int count = localScopeStart;
            while (groupsStack.Count > count)
            {
                EndGroup();
            }
        }

        private void EndAll()
        {
            while (groupsStack.Count > 0)
            {
                EndGroup();
            }
        }

        private void EndGroup()
        {
            if (groupsStack.Count > 0)
            {
                InspectorLayoutGroup group = groupsStack.Pop();
                if (currentPath.Count > 0)
                    currentPath.RemoveAt(currentPath.Count - 1);

                if (group.cachedLocalScopeStart.HasValue)
                    localScopeStart = group.cachedLocalScopeStart.Value;
                if (ScopeVisible)
                {
                    if (group.data.Type == GroupType.Vertical)
                    {
                        EditorGUILayout.EndVertical();
                        if (!group.data.Style.HasFlag(GroupStyle.Box))
                        {
                            if (groupsStack.Count <= 0
                                || groupsStack.Peek().data.Type != GroupType.Horizontal)
                                EditorGUILayout.Space(MarkupGUI.SpaceAfterGroup);
                        }
                    }

                    if (group.data.Type == GroupType.Horizontal)
                        EditorGUILayout.EndHorizontal();

                    if (group.data.Type == GroupType.TabScope)
                        EditorGUILayout.EndVertical();

                    if (group.cachedPrefsPrefix != null)
                        prefsPrefix = group.cachedPrefsPrefix;
                    if (group.cachedIndent.HasValue)
                        EditorGUI.indentLevel = group.cachedIndent.Value;
                    if (group.cachedLabelWidth.HasValue)
                        EditorGUIUtility.labelWidth = group.cachedLabelWidth.Value;
                    if (group.cachedActiveTab != null)
                        activeTabName = group.cachedActiveTab;
                    if (group.cachedHierarchyMode.HasValue)
                        EditorGUIUtility.hierarchyMode = group.cachedHierarchyMode.Value;

                    group.cachedPrefsPrefix = null;
                    group.cachedActiveTab = null;
                    group.cachedIndent = null;
                    group.cachedLabelWidth = null;
                    group.cachedHierarchyMode = null;
                }
            }
        }
    }
}

