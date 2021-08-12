using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MarkupAttributes.Editor
{
    public static class ShaderAttributesParser
    {
        public static string GetKeyword(string arg, out bool isGlobal)
        {
            string[] s = arg.Split(' ');
            if (s.Length < 2)
            {
                isGlobal = false;
                return arg;
            }
            else
            {
                isGlobal = s[0] == "G";
                return s[1];
            }
        }

        public static bool GetDrawSystemPropertiesAttribute(string[][] allAttributes)
        {
            for (int i = 0; i < allAttributes.Length; i++)
            {
                var attributes = allAttributes[i];
                foreach (var attribute in attributes)
                {
                    if (ParseAttribute(attribute, "DrawSystemProperties"))
                        return true;
                }
            }
            return false;
        }
        public static CompactTextureMode? GetCompactTextureAttribute(string[] attributes)
        {
            CompactTextureMode? mode = null;
            for (int i = 0; i < attributes.Length; i++)
            {
                mode = GetCompactTextureAttribute(attributes[i]);
                if (mode != null)
                    break;
            }
            return mode;
        }

        private static CompactTextureMode? GetCompactTextureAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "CompactTexture", 0, out string[] args);
            if (!valid)
                return null;
            if (args != null && args.Length > 0)
            {
                if (args[0] == "UniformScaleOnly")
                    return CompactTextureMode.UniformScaleOnly;
                if (args[0] == "ScaleOnly")
                    return CompactTextureMode.ScaleOnly;
            }
            return CompactTextureMode.Default;
        }

        public static PropertyLayoutData[] GetLayoutData(string[][] allAttributes,
            MaterialPropertiesWrapper propertiesWrapper, Material targetMaterial)
        {
            var props = propertiesWrapper.value;
            var output = new PropertyLayoutData[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                bool isPropertyHidden = false;

                var groupAttributes = GetLayoutGroupAttributes(allAttributes[i]);
                var groups = new List<InspectorLayoutGroup>();
                foreach (var groupAttribute in groupAttributes)
                {
                    var group = CreateGroupFromAttribute(ref isPropertyHidden, groupAttribute,
                        i, propertiesWrapper, targetMaterial);
                    if (group != null)
                        groups.Add(group);
                }
                output[i] = new PropertyLayoutData(
                    groups, GetEndGroupAttribute(allAttributes[i]));
                output[i].hide = isPropertyHidden;
            }

            return output;
        }

        private static InspectorLayoutGroup CreateGroupFromAttribute(ref bool isHidden,
            LayoutGroupAttribute attribute, int index, 
            MaterialPropertiesWrapper allProperties, Material targetMaterial)
        {
            ConditionWrapper conditionWrapper = null;
            if (attribute.HasCondition)
            {
                conditionWrapper = ConditionWrapper.Create(attribute.Condition,
                    attribute.IsConditionInverted, allProperties, targetMaterial);
                if (conditionWrapper == null) return null;
            }

            TogglableValueWrapper togglableValueWrapper = null;
            if (attribute.Toggle)
            {
                if (attribute.ShaderKeyword != null)
                {
                    togglableValueWrapper = TogglableValueWrapper.Create(index, allProperties, targetMaterial, attribute.ShaderKeyword);
                    isHidden = true;
                }
                else
                {
                    togglableValueWrapper = TogglableValueWrapper.Create(index, allProperties);
                    isHidden = true;
                }
                
                if (togglableValueWrapper == null)
                    return null;
            }

            return new InspectorLayoutGroup(attribute, conditionWrapper, togglableValueWrapper);
        }

        private static EndGroupAttribute GetEndGroupAttribute(string[] attributes)
        {
            foreach (var attribute in attributes)
            {
                if (ParseAttribute(attribute, "EndGroup", 0, out string[] args))
                {
                    EndGroupAttribute a;
                    if (args != null && args.Length > 0)
                        a = new EndGroupAttribute(args[0]);
                    else
                        a = new EndGroupAttribute();
                    return a;
                }
            }
            return null;
        }

        private static LayoutGroupAttribute[] GetLayoutGroupAttributes(string[] attributes)
        {
            var groups = new List<LayoutGroupAttribute>();
            var temp = new List<LayoutGroupAttribute>();
            for (int i = 0; i < attributes.Length; i++)
            {
                temp.Clear();
                temp.Add(GetHideIfGroupAttribute(attributes[i]));
                temp.Add(GetDisableIfGroupAttribute(attributes[i]));
                temp.Add(GetTabScopeAttribute(attributes[i]));
                temp.Add(GetTabAttribute(attributes[i]));
                temp.Add(GetVerticalGroupAttribute(attributes[i]));
                temp.Add(GetHorizontalGroupAttribute(attributes[i]));
                temp.Add(GetTitleGroupAttribute(attributes[i]));
                temp.Add(GetFoldoutGroupAttribute(attributes[i]));
                temp.Add(GetToggleGroupAttribute(attributes[i]));
                temp.Add(GetBoxAttribute(attributes[i]));

                foreach (var g in temp)
                {
                    if (g != null)
                        groups.Add(g);
                }
            }
            return groups.ToArray();
        }

        private static HideIfGroupAttribute GetHideIfGroupAttribute(string attribute)
        {
            string[] args;
            bool valid = ParseAttribute(attribute, "HideIfGroup", 2, out args);
            if (valid)
                return new HideIfGroupAttribute(GetPath(args[0]), args[1]);
            valid = ParseAttribute(attribute, "ShowIfGroup", 2, out args);
            if (valid)
                return new ShowIfGroupAttribute(GetPath(args[0]), args[1]);
            return null;
        }

        private static DisableIfGroupAttribute GetDisableIfGroupAttribute(string attribute)
        {
            string[] args;
            bool valid = ParseAttribute(attribute, "DisableIfGroup", 2, out args);
            if (valid)
                return new DisableIfGroupAttribute(GetPath(args[0]), args[1]);
            valid = ParseAttribute(attribute, "EnableIfGroup", 2, out args);
            if (valid)
                return new EnableIfGroupAttribute(GetPath(args[0]), args[1]);
            return null;
        }

        private static TabScopeAttribute GetTabScopeAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "TabScope", 2, out string[] args);
            if (valid)
            {
                if (args.Length < 3)
                    return new TabScopeAttribute(GetPath(args[0]), GetTabs(args[1]));
                else
                    return new TabScopeAttribute(GetPath(args[0]), GetTabs(args[1]), GetBool(args[2]));
            }
            return null;
        }

        private static TabAttribute GetTabAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "Tab", 1, out string[] args);
            if (valid)
                return new TabAttribute(GetPath(args[0]));
            return null;
        }

        private static VerticalGroupAttribute GetVerticalGroupAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "VerticalGroup", 1, out string[] args);
            if (valid)
            {
                return new VerticalGroupAttribute(GetPath(args[0]));
            }  
            return null;
        }

        private static HorizontalGroupAttribute GetHorizontalGroupAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "HorizontalGroup", 2, out string[] args);
            if (valid)
                return new HorizontalGroupAttribute(GetPath(args[0]), GetFloat(args[1]));
            return null;
        }

        private static FoldoutAttribute GetFoldoutGroupAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "Foldout", 1, out string[] args);
            if (valid)
            {
                if (args.Length < 2)
                    return new FoldoutAttribute(GetPath(args[0]));
                return new FoldoutAttribute(GetPath(args[0]), GetEnum<MarkupBodyStyle>(args[1]));
            }
            return null;
        }

        private static ToggleGroupAttribute GetToggleGroupAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "ToggleGroup", 1, out string[] args);
            if (valid)
            {
                if (args.Length < 2)
                    return ToggleGroupAttribute.CreateForShader(GetPath(args[0]), false, null);
                if (args.Length < 3)
                    return ToggleGroupAttribute.CreateForShader(GetPath(args[0]), GetBool(args[1]), null);
                return ToggleGroupAttribute.CreateForShader(GetPath(args[0]), GetBool(args[1]), args[2]);
            }
            return null;
        }

        private static TitleGroupAttribute GetTitleGroupAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "TitleGroup", 1, out string[] args);
            if (valid)
            {
                return new TitleGroupAttribute(GetPath(args[0]));
            }
            return null;
        }

        private static BoxAttribute GetBoxAttribute(string attribute)
        {
            bool valid = ParseAttribute(attribute, "Box", 1, out string[] args);
            if (valid)
            {
                if (args.Length < 2)
                    return new BoxAttribute(GetPath(args[0]));
                return new BoxAttribute(GetPath(args[0]), GetBool(args[1]));
            }
            return null;
        }

        private static string GetPath(string arg)
        {
            arg = arg.Replace(' ', '/');
            arg = arg.Replace('_', ' ');
            return arg;
        }

        private static string GetTabs(string arg)
        {
            arg = arg.Replace(' ', '|');
            arg = arg.Replace('_', ' ');
            return arg;
        }

        private static float GetFloat(string arg) => float.Parse(arg, CultureInfo.InvariantCulture.NumberFormat);

        private static bool GetBool(string arg) => arg == "true";

        private static T GetEnum<T>(string arg) where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            foreach (var value in values)
            {
                if (Enum.GetName(typeof(T), value) == arg)
                    return value;
            }
            return values.First();
        }

        private static bool ParseAttribute(string attribute, string attributeName)
        {
            return ParseAttribute(attribute, attributeName, 0, out _);
        }

        private static bool ParseAttribute(string attribute, string attributeName, int minArgsCount, out string[] args)
        {
            args = null;
            int argStartIndex = attribute.IndexOf('(');
            if (argStartIndex < 0 && minArgsCount > 0)
                return false;
            if (!(argStartIndex < 0 ? attribute : attribute.Substring(0, argStartIndex)).Equals(attributeName))
                return false;

            if (argStartIndex < 0)
                return true;

            string argument = attribute.Substring(argStartIndex + 1, attribute.Length - argStartIndex - 1);
            argument = argument.Trim(')');
            if (argument.Length < 1)
                return !(minArgsCount > 0);

            args = argument.Split(',');
            if (args.Length < minArgsCount)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim();
            }
            return true;
        }
    }
}
