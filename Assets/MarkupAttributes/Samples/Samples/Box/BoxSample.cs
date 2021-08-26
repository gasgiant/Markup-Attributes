using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class BoxSample : SamplesBehaviour
    {
        [Box("Labeled Box")]
        public int one;
        public int two;
        public int three;

        [Box("Unlabeled Box", labeled: false)]
        public int four;
        public int five;
        public int six;
    }
}

