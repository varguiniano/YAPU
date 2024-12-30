using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for a button that displays a monster relationship in the dex.
    /// </summary>
    public class MonsterRelationshipButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Icon that represents the relation.
        /// </summary>
        [SerializeField]
        private Image RelationIcon;

        /// <summary>
        /// Text that represents the relation.
        /// </summary>
        [SerializeField]
        private TMP_Text RelationText;

        /// <summary>
        /// Empty sprite to display when using the relation text.
        /// </summary>
        [SerializeField]
        private Sprite EmptySprite;

        /// <summary>
        /// Icon for the monster.
        /// </summary>
        [SerializeField]
        private Image MonsterIcon;

        /// <summary>
        /// Name of the monster.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro MonsterName;

        /// <summary>
        /// Relation of sprites to display based on the icon type.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<DexMonsterRelationshipData.IconType, Sprite> SpritesToDisplay;

        /// <summary>
        /// Set the relation into this button.
        /// </summary>
        public void SetRelationship(DexMonsterRelationshipData relationshipData)
        {
            switch (relationshipData.Mode)
            {
                case DexMonsterRelationshipData.RelationShipDisplayType.PresetIcon:
                    RelationIcon.sprite = SpritesToDisplay[relationshipData.PresetIcon];
                    RelationText.SetText("");
                    break;
                case DexMonsterRelationshipData.RelationShipDisplayType.Icon:
                    RelationIcon.sprite = relationshipData.Icon;
                    RelationText.SetText("");
                    break;
                case DexMonsterRelationshipData.RelationShipDisplayType.Text:
                    RelationIcon.sprite = EmptySprite;
                    RelationText.SetText(relationshipData.Text);
                    break;
                default:
                    RelationIcon.sprite = EmptySprite;
                    RelationText.SetText("");
                    break;
            }

            DataByFormEntry monsterData = relationshipData.Species[relationshipData.Form];

            if (relationshipData.Form.IsShiny)
                MonsterIcon.sprite = monsterData.HasGenderVariations && relationshipData.Gender == MonsterGender.Male
                                         ? monsterData.IconShinyMale
                                         : monsterData.IconShiny;
            else
                MonsterIcon.sprite = monsterData.HasGenderVariations && relationshipData.Gender == MonsterGender.Male
                                         ? monsterData.IconMale
                                         : monsterData.Icon;

            MonsterName.SetValue(relationshipData.Species.LocalizableName);
        }

        /// <summary>
        /// Class used for DI.
        /// </summary>
        public class Factory : GameObjectFactory<MonsterRelationshipButton>
        {
        }
    }
}