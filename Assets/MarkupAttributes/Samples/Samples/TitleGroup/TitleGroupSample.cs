using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class TitleGroupSample : SamplesBehaviour
    {
        [TitleGroup("Title Group")]
        public int one;
        public int two;
        public int three;

        [TitleGroup("Title Group With a Box", boxContent: true)]
        public int four;
        public int five;
        public int six;
    }
}
