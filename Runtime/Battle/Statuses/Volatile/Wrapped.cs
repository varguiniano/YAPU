using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Wrapped status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Wrapped", fileName = "Wrapped")]
    public class Wrapped : BindingVolatileStatus
    {
        /// <summary>
        /// Percentage of HP drain.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPDrain = 1f / 8f;

        /// <summary>
        /// Percentage of HP drain if affected by binding band.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPDrainIfAffectedByBindingBand = 1f / 6f;

        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Animation to play when the move is used.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Dictionary of the battlers that are being wrapped by other battlers.
        /// If the original wrapper leaves the battle, the wrapped battler is freed.
        /// </summary>
        private readonly Dictionary<Battler, Battler> battlersBeingWrappedByBattlers = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need. 0 must be if affected by binding band.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return base.OnAddStatus(battleManager, battler, extraData);

            Battler wrapper = (Battler) extraData[1];

            if (wrapper == null)
            {
                Logger.Error("No wrapper was provided!");
                yield break;
            }

            battlersBeingWrappedByBattlers[battler] = wrapper;
        }

        /// <summary>
        /// Have the vortex damage the mon.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            if (!battleManager.Battlers.IsBattlerFighting(battleManager.Battlers
                                                                       .GetTypeAndIndexOfBattler(battlersBeingWrappedByBattlers
                                                                            [battler])))
                yield break;

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            BasicSpriteAnimation wrap =
                Instantiate(AnimationPrefab, battleManager.GetMonsterSprite(type, index).transform);

            yield return WaitAFrame;

            battleManager.Audio.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return wrap.PlayAnimation(battleManager.BattleSpeed, true);

            DOVirtual.DelayedCall(3, () => Destroy(wrap.gameObject));

            int amount = Mathf.Max((int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager)
                                        * (AffectedByBindingBand.Contains(battler)
                                               ? HPDrainIfAffectedByBindingBand
                                               : HPDrain)),
                                   1);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                -amount,
                                                                playAudio: false,
                                                                isSecondaryDamage: true);

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield return base.OnRemoveStatus(battleManager, battler, playAnimation);

            // Remove the battler and its wrapper from the dictionary.
            battlersBeingWrappedByBattlers.Remove(battler);
        }

        /// <summary>
        /// Called when another monster is withdrawn from battle.
        /// </summary>
        /// <param name="owner">Owner of the volatile status.</param>
        /// <param name="other">Monster leaving.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnOtherMonsterLeavingBattle(Battler owner,
                                                                Battler other,
                                                                BattleManager battleManager)
        {
            if (battlersBeingWrappedByBattlers[owner] == other)
                battleManager.Statuses.ScheduleRemoveStatus(this, owner);

            yield break;
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            yield return base.OnBattleEnded(battler);

            battlersBeingWrappedByBattlers.Clear();
        }

        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public override bool CanRunAway(Battler battler, BattleManager battleManager, bool showMessages)
        {
            if (showMessages)
                DialogManager.ShowDialog("Status/Volatile/Wrapped/CantRun",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }
    }
}