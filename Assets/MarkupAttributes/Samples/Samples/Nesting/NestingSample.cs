using UnityEngine;

namespace MarkupAttributes.Samples
{
    public class NestingSample : SamplesBehaviour
    {
        [Box("First Group")]
        [SerializeField] private int one;
        // To nest groups, just write their path.
        [TitleGroup("First Group/Nested Group 1")]
        [SerializeField] private int two;
        [SerializeField] private int three;
        // Starting a group closes all groups untill path match.
        [TitleGroup("First Group/Nested Group 2")]
        [SerializeField] private int four;
        [SerializeField] private int five;

        // ./ shortcut opens a group on top of the current one,
        // ../ closes the topmost group and then opens a new one on top.
        [Box("Second Group")]
        [TitleGroup("./Nested Group 1")]
        [SerializeField] private int six;
        [SerializeField] private int seven;
        [TitleGroup("../Nested Group 2")]
        [SerializeField] private int eight;
        [SerializeField] private int nine;
        // [EndGroup] closes the topmost group, or, when provided with a name,
        // closes the named group and all of its children.
        [EndGroup("Second Group")]

        [Box("Third Group")]
        [SerializeField] private int ten;
        [SerializeField] private int eleven;
        // At the end all groups a closed automatically.
    }
}
