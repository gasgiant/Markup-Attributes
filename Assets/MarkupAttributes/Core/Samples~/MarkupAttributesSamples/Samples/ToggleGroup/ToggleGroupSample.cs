using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ToggleGroupSample : SamplesBehaviour
    {
        [ToggleGroup("Toggle Group")]
        public bool boolean;
        public int one;
        public int two;
        public int three;

        [ToggleGroup("Foldable Toggle Group", foldable: true)]
        public bool anotherBoolean;
        public int four;
        public int five;
        public int six;
    }
}
