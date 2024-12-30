using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the leech seed status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/LeechSeed", fileName = "LeechSeedStatus")]
    public class LeechSeedStatus : VolatileStatus
    {
        /// <summary>
        /// Percentage of HP drain.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPDrain = 1f / 8f;

        /// <summary>
        /// Reference to the absorb audio.
        /// </summary>
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Size of the particles over time.
        /// </summary>
        [SerializeField]
        private AnimationCurve ParticlesLifeOverTime;

        /// <summary>
        /// Dictionary that keeps track of the the monsters infected and the position they are being leeched by.
        /// </summary>
        private readonly Dictionary<Battler, (BattlerType, int)> targets = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">[0] target type. [1] target index.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            targets[battler] = ((BattlerType) extraData[0], (int) extraData[1]);

            return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Have the original position caster of the seed drain the battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            if (!targets.ContainsKey(battler))
            {
                Logger.Error("This battler is not registered as leeched!");
                yield break;
            }

            (BattlerType Type, int Index) battlerData = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battlerData);

            (BattlerType Type, int Index) targetData = targets[battler];
            BattleMonsterSprite targetSprite = battleManager.GetMonsterSprite(targetData);
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

            if (!battleManager.Battlers.IsBattlerFighting(battlerData)
             || !battleManager.Battlers.IsBattlerFighting(targetData))
                yield break;

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            CoroutineRunner.RunRoutine(sprite.FXAnimator.PlayAbsorb(1.2f / battleManager.BattleSpeed,
                                                                    targetSprite.transform.position,
                                                                    sprite.transform,
                                                                    spawnRadius: .2f,
                                                                    sizeOverLifetime: ParticlesLifeOverTime));

            DOVirtual.DelayedCall(1.4f, () => targetSprite.FXAnimator.PlayBoost(battleManager.BattleSpeed, false));

            int amount = Mathf.Max((int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager) * HPDrain),
                                   1);

            int hpDrained = 0;

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                battlerData.Type,
                                                                battlerData.Index,
                                                                -amount,
                                                                playAudio: false,
                                                                isSecondaryDamage: true,
                                                                finished: (drained, _) => hpDrained = -drained);

            hpDrained = (int) (hpDrained * target.CalculateDrainHPMultiplier(battleManager, battler));

            hpDrained = (int) (hpDrained * battler.CalculateDrainerDrainHPMultiplier(battleManager, target, false));

            if (hpDrained > 0 && !target.CanHeal(battleManager)) yield break;

            yield return battleManager.BattlerHealth.ChangeLife(targetData.Type,
                                                                targetData.Index,
                                                                battlerData.Type,
                                                                battlerData.Index,
                                                                hpDrained);

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
            targets.Remove(battler);

            yield return base.OnRemoveStatus(battleManager, battler, playAnimation);
        }

        /// <summary>
        /// Called when this status is passed from one battler to another.
        /// For example with Baton Pass.
        /// </summary>
        /// <param name="oldOwner">Old owner of the status.</param>
        /// <param name="newOwner">New owner of the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override void OnMonsterChanged(Battler oldOwner, Battler newOwner, BattleManager battleManager)
        {
            base.OnMonsterChanged(oldOwner, newOwner, battleManager);

            targets[newOwner] = targets[oldOwner];
            targets.Remove(oldOwner);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            targets.Remove(battler);

            yield return base.OnBattleEnded(battler);
        }
    }
}