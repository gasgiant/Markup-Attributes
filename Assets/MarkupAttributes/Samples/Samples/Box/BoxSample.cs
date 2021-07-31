using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class BoxSample : SamplesBehaviour
    {
        [Box("Unlabeled Box")]
        public int one;
        public int two;
        public int three;

        [Box("Labeled Box", labeled: true)]
        public int four;
        public int five;
        public int six;
    }
}

