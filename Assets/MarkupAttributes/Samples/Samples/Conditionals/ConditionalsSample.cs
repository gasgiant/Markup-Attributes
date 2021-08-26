using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ConditionalsSample : SamplesBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private int one;

        [Header("Disabled If One Is Even")]
        [DisableIf(nameof(IsOneEvenProperty))]
        [SerializeField] private int two;
        [DisableIf(nameof(IsOneEvenProperty))]
        [SerializeField] private int three;
        [DisableIf(nameof(IsOneEvenProperty))]
        [SerializeField] private int four;

        [EnableIfGroup("Enabled If One Is Even", nameof(IsOneEvenMethod))]
        [Header("Enabled If One Is Even")]
        [SerializeField] private int five;
        [SerializeField] private int six;
        [SerializeField] private int seven;
        [EndGroup]

        [Space(20)]
        [SerializeField] private bool boolean;
        [Header("Hidden If Boolean")]
        [HideIf(nameof(boolean))]
        [SerializeField] private int eight;
        [HideIf(nameof(boolean))]
        [SerializeField] private int nine;
        [HideIf(nameof(boolean))]
        [SerializeField] private int ten;

        [ShowIfGroup("Shown If Boolean", nameof(boolean))]
        [Header("Shown If Boolean")]
        [SerializeField] private int eleven;
        [SerializeField] private int twelve;
        [SerializeField] private int thirteen;

        [Header("ReadOnly")]
        [ReadOnly]
        [SerializeField] private int fourteen;

        private bool IsOneEvenProperty => one % 2 == 0;
        private bool IsOneEvenMethod() => one % 2 == 0;
    }
}
