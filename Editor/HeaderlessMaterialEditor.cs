using UnityEditor;
using UnityEditorInternal;

namespace MarkupAttributes.Editor
{
    internal class HeaderlessMaterialEditor : MaterialEditor
    {
        protected override void OnHeaderGUI()
        {
            InternalEditorUtility.SetIsInspectorExpanded(target, true);
        }
    }
}
