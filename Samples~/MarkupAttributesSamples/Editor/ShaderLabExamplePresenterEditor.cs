using MarkupAttributes.Editor;
using UnityEditor;

namespace MarkupAttributes.Samples
{
    [CustomEditor(typeof(ShaderLabExamplePresenter))]
    public class ShaderLabExamplePresenterEditor : MarkedUpEditor
    {
        public override void OnInspectorGUI()
        {
            using (new MarkupGUI.ScriptPropertyScope(false))
            {
                DrawMarkedUpInspector();
            }
        }
    }
}
