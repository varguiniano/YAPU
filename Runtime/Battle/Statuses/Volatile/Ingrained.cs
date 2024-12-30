using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Ingrained status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Ingrained", fileName = "Ingrained")]
    public class Ingrained : VolatileStatus
    {
        /// <summary>
        /// HP gain of the ingrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPGain = 1f / 16f;

        /// <summary>
        /// Reference to the charging audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Spawn angle of the particles.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector2 SpawnAngle;

        /// <summary>
        /// Animation curve for the charge size.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AnimationCurve Size;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return PlayAnimation(battler, battleManager);

            yield return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            yield return base.OnTickStatus(battleManager, battler);

            if (!battler.CanHeal(battleManager)) yield break;

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return PlayAnimation(battler, battleManager);

            int hpToGain = Mathf.Max((int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager) * HPGain),
                                     1);

            hpToGain = (int) (hpToGain * battler.CalculateDrainHPMultiplier(battleManager, null));

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                hpToGain,
                                                                playAudio: false);
        }

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator PlayAnimation(Battler battler, BattleManager battleManager)
        {
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battler);

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return new WaitForSeconds(1.1f / battleManager.BattleSpeed);

            yield return sprite.FXAnimator.PlayAbsorb(1.3f / battleManager.BattleSpeed,
                                                      sprite.transform.position,
                                                      spawnAngle: SpawnAngle,
                                                      spawnRadius: .5f,
                                                      sizeOverLifetime: Size);

            yield return sprite.FXAnimator.PlayBoostRoutine(battleManager.BattleSpeed);
        }

        /// <summary>
        /// Does this status ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <returns>True if this status forces the monster to be grounded and true if this status prevents grounding.</returns>
        public override (bool, bool) IsGrounded(Battler battler) => (true, false);

        /// <summary>
        /// Check if the battler can switch.
        /// </summary>
        /// <param name="battler">Battler with the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public override bool CanSwitch(Battler battler,
                                       BattleManager battleManager,
                                       BattlerType userType,
                                       int userIndex,
                                       Move userMove,
                                       Item item,
                                       bool itemBelongsToUser,
                                       bool showMessages)
        {
            (BattlerType ownType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            if (ownType == userType)
                return base.CanSwitch(battler,
                                      battleManager,
                                      userType,
                                      userIndex,
                                      userMove,
                                      item,
                                      itemBelongsToUser,
                                      showMessages);

            if (showMessages)
                DialogManager.ShowDialog("Status/Volatile/Ingrained/CantSwitch",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }
    }
}