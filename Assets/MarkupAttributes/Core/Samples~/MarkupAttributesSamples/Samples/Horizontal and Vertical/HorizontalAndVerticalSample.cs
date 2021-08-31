using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class HorizontalAndVerticalSample : SamplesBehaviour
    {
        [HorizontalGroup("Split", labelWidth: 50)]
        [VerticalGroup("./Left")]
        public int one;
        public int two;
        public int three;

        [VerticalGroup("../Right")]
        public int four;
        public int five;
        public int six;
    }
}
