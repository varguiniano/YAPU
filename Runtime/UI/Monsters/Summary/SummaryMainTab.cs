using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.UI.Items;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.Summary
{
    /// <summary>
    /// Main tab of the summary screen.
    /// </summary>
    public class SummaryMainTab : SummaryTab
    {
        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro SpeciesText;

        /// <summary>
        /// Reference to the first type badge.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TypeBadge TypeBadge1;

        /// <summary>
        /// Reference to the second type badge.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TypeBadge TypeBadge2;

        /// <summary>
        /// Reference to the friendship text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Friendship;

        /// <summary>
        /// Reference to the friendship bar.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Slider FriendshipBar;

        /// <summary>
        /// Reference to the nature text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro NatureText;

        /// <summary>
        /// Reference to the total xp text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text TotalXP;

        /// <summary>
        /// Reference to the total xp text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TweenableSlider XPBar;

        /// <summary>
        /// Reference to the needed xp text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text NeededXP;

        /// <summary>
        /// Reference to the held item description.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private ItemDescription HeldItem;

        /// <summary>
        /// Reference to the XP lookup table.
        /// </summary>
        [Inject]
        private ExperienceLookupTable lookupTable;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Set the monster data in this tab.
        /// </summary>
        /// <param name="monster">Monster reference.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        public override void SetData(MonsterInstance monster, BattleManager battleManager)
        {
            SpeciesText.SetValue(monster.Species.LocalizableName);

            (MonsterType firstType, MonsterType secondType) = monster.GetTypes(settings);

            TypeBadge1.SetType(firstType);
            TypeBadge2.SetType(secondType);
            Friendship.SetText(monster.Friendship.ToString());
            FriendshipBar.value = monster.Friendship;
            NatureText.SetValue(monster.StatData.Nature.LocalizableName);

            int nextLevelXP = monster.GetExperienceForNextLevel(lookupTable);

            TotalXP.SetText((lookupTable.GetBaseExperienceForLevel(monster.FormData.GrowthRate, monster.StatData.Level)
                           + monster.StatData.CurrentLevelExperience).ToString());

            XPBar.SetValue(1, monster.StatData.CurrentLevelExperience, nextLevelXP);
            NeededXP.SetText((nextLevelXP - monster.StatData.CurrentLevelExperience).ToString());

            HeldItem.SetItem(monster.HeldItem);
        }
    }
}