using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class TabsSample : SamplesBehaviour
    {
        [TabScope("Tab Scope", "Left|Middle|Right", box: true)]
        [Tab("./Left")]
        public int one;
        public int two;
        public int three;

        [Tab("../Middle")]
        public int four;
        public int five;
        public int six;

        [Tab("../Right")]
        public int seven;
        public int eight;
        public int nine;
    }
}
