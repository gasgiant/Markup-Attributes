using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class ReadOnlySample : SamplesBehaviour
    {
        [ReadOnly]
        public int one;

        [ReadOnlyGroup("Read Only Group")]
        [Header("Read Only Group")]
        public int two;
        public int three;
        public int four;
    }
}
