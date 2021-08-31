using UnityEditor;
using MarkupAttributes.Editor;

namespace MarkupAttributes.Samples
{
    [CustomEditor(typeof(CharacterSheet)), CanEditMultipleObjects]
    public class CharacterSheetEditor : MarkedUpEditor
    {
    }
}
