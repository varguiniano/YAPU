using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// Paralysis monster status.
    /// TODO: Interaction with Magic Guard.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Paralysis", fileName = "Paralysis")]
    public class Paralysis : Status
    {
        /// <summary>
        /// Chance to fully paralyze.
        /// </summary>
        [FoldoutGroup("Effect")]
        [PropertyRange(0, 1)]
        [SerializeField]
        private float FullParalysisChance = .25f;

        /// <summary>
        /// Multiplier to apply to speed.
        /// </summary>
        [FoldoutGroup("Effect")]
        [PropertyRange(0, 1)]
        [SerializeField]
        private float SpeedMultiplier = .5f;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Reference to the audio for the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Called when the status is added in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        /// <param name="extraData">Not used.</param>
        public override IEnumerator OnStatusAddedInBattle(Battler battler,
                                                          BattleManager battleManager,
                                                          bool ignoresAbilities,
                                                          bool showMessage = true,
                                                          params object[] extraData)
        {
            yield return PlayAnimation(battler, battleManager);

            yield return base.OnStatusAddedInBattle(battler, battleManager, ignoresAbilities, showMessage);
        }

        /// <summary>
        /// Do not show message on tick.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusTickInBattle(Battler battler,
                                                         BattleManager battleManager,
                                                         bool showMessage = true) =>
            base.OnStatusTickInBattle(battler, battleManager, false);

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToPerformMove(Battler battler,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Action<bool> finished)
        {
            float chance = battleManager.RandomProvider.Value01();

            if (!(chance <= FullParalysisChance)) yield break;

            yield return PlayAnimation(battler, battleManager);

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager
                                                            .Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            finished?.Invoke(false);
        }

        /// <summary>
        /// Called when calculating a stat of the monster that has this status.
        /// </summary>
        /// <param name="monster">Monster that has the status.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager, if in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            stat == Stat.Speed
         && battleManager != null
         && ((Battler) monster).IsAffectedByParalysisSpeedReduction(battleManager)
                ? SpeedMultiplier
                : base.OnCalculateStat(monster, stat, battleManager);

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator PlayAnimation(Battler battler, BattleManager battleManager)
        {
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battler);

            BasicSpriteAnimation animation = Instantiate(AnimationPrefab, sprite.Pivot);

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return animation.PlayAnimation(battleManager.BattleSpeed);

            Destroy(animation.gameObject);
        }
    }
}