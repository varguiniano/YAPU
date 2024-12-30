using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Class representing relationship data between two monsters to be displayed on the dex.
    /// </summary>
    public class DexMonsterRelationshipData
    {
        /// <summary>
        /// Mode of displaying the relationship.
        /// </summary>
        public RelationShipDisplayType Mode;

        /// <summary>
        /// Sprite to display.
        /// </summary>
        public IconType PresetIcon;

        /// <summary>
        /// Sprite to display.
        /// </summary>
        public Sprite Icon;

        /// <summary>
        /// Text to display.
        /// </summary>
        public string Text;

        /// <summary>
        /// Monster to display.
        /// </summary>
        public MonsterEntry Species;

        /// <summary>
        /// Form to display.
        /// </summary>
        public Form Form;

        /// <summary>
        /// Gender to display.
        /// </summary>
        public MonsterGender Gender;

        /// <summary>
        /// Localizable key for the description.
        /// </summary>
        public string LocalizableDescriptionKey;

        /// <summary>
        /// Enumeration with the two ways of displaying the relationship.
        /// </summary>
        public enum RelationShipDisplayType
        {
            PresetIcon,
            Icon,
            Text
        }

        /// <summary>
        /// Enumeration of the types of icons that can be displayed.
        /// </summary>
        public enum IconType
        {
            NormalBreeding = 0,
            DittoBreeding = 1,
            FemaleGender = 2,
            MaleGender = 3,
        }
    }
}