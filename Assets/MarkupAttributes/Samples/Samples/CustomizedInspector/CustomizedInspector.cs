using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class CustomizedInspector : MonoBehaviour
    {
        // See CustomizedInspectorEditor.cs for details

        [HorizontalGroup("Split", labelWidth: 50)]
        [Box("./Left", labeled: true)]
        [SerializeField] private float one;
        [SerializeField] private float two;
        [SerializeField] private float three;

        [Box("../Right", labeled: true)]
        [SerializeField] private float four;
        [SerializeField] private float five;
        [SerializeField] private float six;
    }
}
