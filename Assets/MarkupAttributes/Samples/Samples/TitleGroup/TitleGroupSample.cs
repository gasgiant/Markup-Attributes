using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class TitleGroupSample : SamplesBehaviour
    {
        [TitleGroup("Title Group")]
        public int one;
        public int two;
        public int three;
    }
}
