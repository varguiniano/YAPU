using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// Poison monster status.
    /// TODO: What about the corrosion ability?
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Poison", fileName = "Poison")]
    public class Poison : Status
    {
        /// <summary>
        /// HP reduction of the poison.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPReduction = 1f / 8f;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Reference to the poisoned audio.
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
        /// Called when the status is ticked in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusTickInBattle(Battler battler,
                                                         BattleManager battleManager,
                                                         bool showMessage = true)
        {
            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return PlayAnimation(battler, battleManager);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                CalculatePoisonDamage(battler, battleManager),
                                                                playAudio: false,
                                                                isSecondaryDamage: true);

            yield return base.OnStatusTickInBattle(battler, battleManager, showMessage);
        }

        /// <summary>
        /// Calculate the poison damage each tick.
        /// </summary>
        /// <param name="battler">The battler infected.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>The value of the damage to take.</returns>
        protected virtual int CalculatePoisonDamage(Battler battler, BattleManager battleManager) =>
            Mathf.RoundToInt(-Mathf.Max((int) (MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager)
                                             * HPReduction),
                                        1)
                           * battler.CalculatePoisonDamageMultiplier(battleManager));

        /// <summary>
        /// Play the poison animation.
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