using UnityEngine;

namespace MarkupAttributes.Samples
{

    //[CreateAssetMenu(menuName = "MarkupAttributes Samples/Character Sheet", fileName = "New Character Sheet")]
    public class CharacterSheet : ScriptableObject
    {
        [Box("Main", labeled: true)]
        public string characterName;
        public Race race;
        public Class characterClass;
        public Alignment alignment;
        public int level;

        [TitleGroup("./Stats", space: 7)]
        public int strength;
        public int dexterity;
        public int constitution;
        public int intelligence;
        public int wisdom;
        public int charisma;


        [TabScope("Tab Scope", "Personality|Equipment|Spells", box: true)]
        [Tab("./Personality")]
        [TextArea]
        public string traits;
        [TextArea]
        public string ideals;
        [Tab("../Equipment")]
        public string[] weapons;
        public string[] potions;
        [TextArea]
        public string other;

        [Tab("../Spells")]
        public string[] fivteen;
        [EndGroup("Tab Scope")]

        [InlineEditor(InlineEditorMode.Box)]
        public Material material;

        public enum Race { Human, Elf, Dwarf, Halfing, Tiefling }
        public enum Class { Warrior, Barbarian, Ranger, Rogue, Sorcerer, Warlock }

        public enum Alignment
        {
            LawfulGood,
            NeutralGood,
            ChaoticGood,
            LawfulNeutral,
            TrueNeutral,
            ChaoticNeutral,
            LawfulEvil,
            NeutralEvil,
            ChaoticEvial
        }
    }
}
