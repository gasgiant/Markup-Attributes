using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ConditionalsSample : SamplesBehaviour
    {
        
        private bool IsOneEvenProperty => one % 2 == 0;
        private bool IsOneEvenMethod() => one % 2 == 0;

        [Range(0, 1)]
        public int one;

        [Header("Disabled If One Is Even")]
        [DisableIf(nameof(IsOneEvenProperty))]
        public int two;
        [DisableIf(nameof(IsOneEvenProperty))]
        public int three;

        [EnableIfGroup("Enabled If One Is Even", nameof(IsOneEvenMethod))]
        [Header("Enabled If One Is Even")]
        public int four;
        public int five;
        [EndGroup]

        [Space(20)]
        public bool boolean;
        [Header("Hidden If Boolean")]
        [HideIf(nameof(boolean))]
        public int six;
        [HideIf(nameof(boolean))]
        public int seven;

        [ShowIfGroup("Shown If Boolean", nameof(boolean))]
        [Header("Shown If Boolean")]
        public int eight;
        public int nine;
        [EndGroup]
        

        [Header("Shown If Enum Value")]
        public SomeEnum state;
        [ShowIf(nameof(state), SomeEnum.Foo)]
        public int ten;
        [ShowIf(nameof(state), SomeEnum.Foo)]
        public int eleven;
        [ShowIf(nameof(GetState), SomeEnum.Bar)]
        public int twelve;
        [ShowIf(nameof(GetState), SomeEnum.Bar)]
        public int thirteen;

        public enum SomeEnum { Foo, Bar }
        private SomeEnum GetState() => state;
    }
}
