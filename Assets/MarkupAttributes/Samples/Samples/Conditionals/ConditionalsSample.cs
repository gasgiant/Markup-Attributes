using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ConditionalsSample : SamplesBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private int one;

        [DisableIfGroup("Disabled If One Is Even", nameof(IsOneEvenProperty))]
        [Header("Disabled If One Is Even")]
        [SerializeField] private int two;
        [SerializeField] private int three;
        [SerializeField] private int four;

        [EnableIfGroup("Enabled If One Is Even", nameof(IsOneEvenMethod))]
        [Header("Enabled If One Is Even")]
        [SerializeField] private int five;
        [SerializeField] private int six;
        [SerializeField] private int seven;

        [Space(20)]
        [SerializeField] private bool boolean;
        [HideIfGroup("Hidden If Boolean", nameof(boolean))]
        [Header("Hidden If Boolean")]
        [SerializeField] private int eight;
        [SerializeField] private int nine;
        [SerializeField] private int ten;

        [ShowIfGroup("Shown If Boolean", nameof(boolean))]
        [Header("Shown If Boolean")]
        [SerializeField] private int eleven;
        [SerializeField] private int twelve;
        [SerializeField] private int thirteen;

        private bool IsOneEvenProperty => one % 2 == 0;
        private bool IsOneEvenMethod() => one % 2 == 0;
    }
}
