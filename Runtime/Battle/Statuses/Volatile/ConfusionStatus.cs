using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the confusion volatile status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Confusion", fileName = "Confusion")]
    public class ConfusionStatus : VolatileStatus
    {
        /// <summary>
        /// Change of the monster damaging itself.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float Chance;

        /// <summary>
        /// Power of the damage to inflict.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private byte DamagePower;

        /// <summary>
        /// Time for the animation to last.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float AnimationTime;

        /// <summary>
        /// Reference to the confused audio.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private AudioReference ConfusedAudio;

        /// <summary>
        /// Reference to the confusion prefab.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private GameObject ConfusionPrefab;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return ConfusionAnimation(battler, battleManager);

            yield return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Don't show anything on tick.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToUseAMove(Battler battler,
                                                      Move move,
                                                      BattleManager battleManager,
                                                      List<(BattlerType Type, int Index)> targets,
                                                      Action<bool> finished)
        {
            yield return ConfusionAnimation(battler, battleManager);

            yield return DialogManager.ShowDialogAndWait("Status/Volatile/Confusion/Start",
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            float confusionChance = battleManager.RandomProvider.Value01();

            if (confusionChance > Chance)
            {
                finished.Invoke(true);
                yield break;
            }

            yield return DealDamage(battler, battleManager);

            finished.Invoke(false);
        }

        /// <summary>
        /// Play the confusion animation.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator ConfusionAnimation(Battler battler, BattleManager battleManager)
        {
            battleManager.AudioManager.PlayAudio(ConfusedAudio, true, battleManager.BattleSpeed);

            (BattlerType type, int index) = battleManager.Battlers
                                                         .GetTypeAndIndexOfBattler(battler);

            GameObject animation = Instantiate(ConfusionPrefab, battleManager.GetMonsterSprite(type, index).Pivot);

            yield return new WaitForSeconds(AnimationTime / battleManager.BattleSpeed);

            Destroy(animation);

            battleManager.AudioManager.StopAudio(ConfusedAudio);
        }

        /// <summary>
        /// Deal the confusion damage. This is very similar to the damage of a physical move,
        /// so the code is copied from the DamageMove class minus the changes for confusion.
        /// Based on: https://bulbapedia.bulbagarden.net/wiki/Stat#Generations_III_onward
        /// And: https://bulbapedia.bulbagarden.net/wiki/Confusion_(status_condition)#Effect
        /// </summary>
        /// <param name="battler">Battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator DealDamage(Battler battler, BattleManager battleManager)
        {
            float attack = MonsterMathHelper.CalculateStat(battler, Stat.Attack, battleManager)
                         * MonsterMathHelper.GetStageMultiplier(battler, Stat.Attack);

            float defense = MonsterMathHelper.CalculateStat(battler, Stat.Defense, battleManager)
                          * MonsterMathHelper.GetStageMultiplier(battler, Stat.Defense);

            float attackDefense = attack / defense;

            float numerator = (2 * battler.StatData.Level / 5f + 2)
                            * DamagePower
                            * attackDefense;

            float baseMultiplier = numerator / 50 + 2;

            float random = battleManager.RandomProvider.Range(.85f, 1);

            float damage = baseMultiplier * random;

            DialogManager.ShowDialog("Status/Volatile/Confusion/DealDamage",
                                     localizableModifiers: false,
                                     acceptInput: false,
                                     modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                     switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.BattlerHealth.ChangeLife(battler, type, index, (int)-damage);

            yield return DialogManager.WaitForDialog;
        }
    }
}