using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ConditionalsSample : SamplesBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private int one;

        [Header("Disabled If One Is Even")]
        [DisableIf(nameof(IsOneEvenProperty))]
        public int two;
        [DisableIf(nameof(IsOneEvenProperty))]
        public int three;
        [DisableIf(nameof(IsOneEvenProperty))]
        public int four;

        [EnableIfGroup("Enabled If One Is Even", nameof(IsOneEvenMethod))]
        [Header("Enabled If One Is Even")]
        public int five;
        public int six;
        public int seven;
        [EndGroup]

        [Space(20)]
        [SerializeField] private bool boolean;
        [Header("Hidden If Boolean")]
        [HideIf(nameof(boolean))]
        public int eight;
        [HideIf(nameof(boolean))]
        public int nine;
        [HideIf(nameof(boolean))]
        public int ten;

        [ShowIfGroup("Shown If Boolean", nameof(boolean))]
        [Header("Shown If Boolean")]
        public int eleven;
        public int twelve;
        public int thirteen;

        private bool IsOneEvenProperty => one % 2 == 0;
        private bool IsOneEvenMethod() => one % 2 == 0;
    }
}
