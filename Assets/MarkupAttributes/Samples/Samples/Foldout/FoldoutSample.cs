using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class FoldoutSample : SamplesBehaviour
    {
        [Foldout("Foldout In A Box")]
        public int one;
        public int two;
        public int three;

        [Foldout("Foldout", false)]
        public int four;
        public int five;
        public int six;
    }
}
