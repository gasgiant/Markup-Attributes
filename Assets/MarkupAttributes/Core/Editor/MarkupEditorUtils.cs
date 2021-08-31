using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace MarkupAttributes.Editor
{
	internal static class MarkupEditorUtils
	{
		public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

		public static SerializedProperty[] GetSerializedObjectProperties(SerializedObject serializedObject)
		{
			List<SerializedProperty> props = new List<SerializedProperty>();
			using (SerializedProperty iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						props.Add(iterator.Copy());
					}
					while (iterator.NextVisible(false));
				}
			}
			return props.ToArray();
		}

		public static SerializedProperty[] GetChildrenProperties(SerializedProperty serializedProperty)
		{
			return GetChildrenOfProperty(serializedProperty).ToArray();
		}

		public static IEnumerable<SerializedProperty> GetChildrenOfProperty(SerializedProperty serializedProperty)
		{
			SerializedProperty currentProperty = serializedProperty.Copy();
			SerializedProperty nextSiblingProperty = serializedProperty.Copy();

			nextSiblingProperty.Next(false);
			if (currentProperty.Next(true))
			{
				do
				{
					if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
						break;

					yield return currentProperty.Copy();
				}
				while (currentProperty.Next(false));
			}
		}


		// from https://github.com/lordofduct/spacepuppy-unity-framework-3.0/blob/master/SpacepuppyUnityFrameworkEditor/EditorHelper.cs
		public static object GetTargetObjectOfProperty(SerializedProperty property)
		{
			if (property == null)
			{
				return null;
			}

			string path = property.propertyPath.Replace(".Array.data[", "[");
			object obj = property.serializedObject.targetObject;
			string[] elements = path.Split('.');

			foreach (var element in elements)
			{
				if (element.Contains("["))
				{
					string elementName = element.Substring(0, element.IndexOf("["));
					int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue_Imp(obj, elementName, index);
				}
				else
				{
					obj = GetValue_Imp(obj, element);
				}
			}

			return obj;
		}

		private static object GetValue_Imp(object source, string name)
		{
			if (source == null)
			{
				return null;
			}

			Type type = source.GetType();

			while (type != null)
			{
				FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (field != null)
				{
					return field.GetValue(source);
				}

				PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (property != null)
				{
					return property.GetValue(source, null);
				}

				type = type.BaseType;
			}

			return null;
		}

		private static object GetValue_Imp(object source, string name, int index)
		{
			IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
			if (enumerable == null)
			{
				return null;
			}

			IEnumerator enumerator = enumerable.GetEnumerator();
			for (int i = 0; i <= index; i++)
			{
				if (!enumerator.MoveNext())
				{
					return null;
				}
			}

			return enumerator.Current;
		}
	}
}
