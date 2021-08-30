using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class MarkedUpFieldSample : SamplesBehaviour
    {
        [MarkedUpField]
        public SomeSerializedClass normal;

        [MarkedUpField(indentChildren: false)]
        public SomeSerializedClass noChildrenIndent;

        [TitleGroup("No Control", contentBox: true)]
        [MarkedUpField(showControl: false, indentChildren: false)]
        public SomeSerializedStruct noControl;
    }

    [System.Serializable]
    public class SomeSerializedClass
    {
        public bool boolean;

        [EnableIfGroup("", nameof(boolean))]
        [TitleGroup("./Enabled If Boolean")]
        public int one;
        public int two;
        public int three;
    }

    [System.Serializable]
    public struct SomeSerializedStruct
    {
        public bool boolean;

        [EnableIfGroup("", nameof(boolean))]
        [TitleGroup("./Enabled If Boolean")]
        public int one;
        public int two;
        public int three;
    }
}
