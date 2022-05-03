using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class MarkedUpFieldSample : SamplesBehaviour
    {
        public SomeSerializedClass normal;

        [MarkedUpField(indentChildren: false)]
        public SomeSerializedClass noChildrenIndent;

        [TitleGroup("No Control", contentBox: true)]
        public SomeSerializedStruct noControl;
    }

    [MarkedUpType]
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

    [MarkedUpType(showControl: false, indentChildren: false)]
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
