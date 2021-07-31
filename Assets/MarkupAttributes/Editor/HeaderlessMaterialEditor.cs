using UnityEditor;
using UnityEditorInternal;

namespace MarkupAttributes.Editor
{
    public class HeaderlessMaterialEditor : MaterialEditor
    {
        protected override void OnHeaderGUI()
        {
            InternalEditorUtility.SetIsInspectorExpanded(target, true);
        }
    }
}
