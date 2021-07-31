using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class FoldoutSample : SamplesBehaviour
    {
        [Foldout("Boxed Foldout")]
        public int one;
        public int two;
        public int three;

        [Foldout("Foldout", boxed: false)]
        public int four;
        public int five;
        public int six;
    }
}
