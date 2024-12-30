using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing a x stat item effect.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/XStatEffect", fileName = "XStatEffect")]
    public class XStatEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// Should we change a stat?
        /// </summary>
        [SerializeField]
        private bool ChangeStat = true;

        /// <summary>
        /// Stat to change.
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(ChangeStat))]
        private Stat StatToChange;

        /// <summary>
        /// Should we change a battle stat?
        /// </summary>
        [SerializeField]
        private bool ChangeBattleStat;

        /// <summary>
        /// Stat to change.
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(ChangeBattleStat))]
        private BattleStat BattleStatToChange;

        /// <summary>
        /// Should we change a battle stat?
        /// </summary>
        [SerializeField]
        private bool ChangeCritical;

        /// <summary>
        /// Stages to change the stat.
        /// </summary>
        [Space]
        [SerializeField]
        [ShowIf("@ChangeStat || ChangeBattleStat || ChangeCritical")]
        [InfoBox("Affects all selected.")]
        private short StagesToChange;

        /// <summary>
        /// Not compatible outside of battle.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item"></param>
        /// <param name="playerCharacter"></param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            false;

        /// <summary>
        /// Check if the effect can be used on a monster in battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="item"></param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler, Item item)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            return battleManager.Battlers.IsBattlerFighting(type, index)
                && ((ChangeStat && battler.StatStage[StatToChange] < 6)
                 || (ChangeBattleStat && battler.BattleStatStage[BattleStatToChange] < 6)
                 || (ChangeCritical && battler.CriticalStage < 6));
        }

        /// <summary>
        /// Apply the life changing effect.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        /// <param name="wasFlung">Was the item flung to this battler?</param>
        public override IEnumerator UseOnBattler(Item item,
                                                 Battler battler,
                                                 BattleManager battleManager,
                                                 YAPUSettings settings,
                                                 ExperienceLookupTable experienceLookupTable,
                                                 ILocalizer localizer,
                                                 Action<bool> finished,
                                                 bool wasFlung = false)
        {
            Logger.Info("Used " + StatToChange + " change item of " + StagesToChange + " on " + battler.Species.name);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            if (ChangeStat)
                yield return battleManager.BattlerStats.ChangeStatStage(type,
                                                                        index,
                                                                        StatToChange,
                                                                        StagesToChange,
                                                                        type,
                                                                        index);

            if (ChangeBattleStat)
                yield return
                    battleManager.BattlerStats.ChangeStatStage(type,
                                                               index,
                                                               BattleStatToChange,
                                                               StagesToChange,
                                                               type,
                                                               index);

            if (ChangeCritical)
                yield return battleManager.BattlerStats.ChangeCriticalStage(type, index, StagesToChange, type, index);

            finished?.Invoke(true);
        }
    }
}