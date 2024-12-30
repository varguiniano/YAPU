using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the Infestation status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/InfestationStatus",
                     fileName = "InfestationStatus")]
    public class InfestationStatus : BindingVolatileStatus
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
        /// Reference to the vortex audio.
        /// </summary>
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the vortex prefab.
        /// </summary>
        [SerializeField]
        private VisualEffect VortexPrefab;

        /// <summary>
        /// Have the vortex damage the mon.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return PlayAnimation(battleManager, battler);

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
        /// Play the effect animation.
        /// </summary>
        public IEnumerator PlayAnimation(BattleManager battleManager, Battler battler)
        {
            VisualEffect vortex = Instantiate(VortexPrefab, battleManager.GetMonsterSprite(battler).transform);

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            vortex.EnableAndPlay();

            yield return new WaitForSeconds(1.8f / battleManager.BattleSpeed);

            vortex.Stop();

            DOVirtual.DelayedCall(3, () => Destroy(vortex.gameObject));
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
                DialogManager.ShowDialog("Status/Volatile/Infestation/CantRun",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: battler.GetNameOrNickName(battleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }
    }
}