using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// Burn monster status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Burn", fileName = "Burn")]
    public class Burn : Status
    {
        /// <summary>
        /// HP reduction of the poison.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPReduction = 1f / 16f;

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
                                                                -Mathf.Max((int) (MonsterMathHelper
                                                                                          .CalculateStat(battler,
                                                                                                    Stat.Hp,
                                                                                                    battleManager)
                                                                                     * HPReduction),
                                                                           1),
                                                                playAudio: false,
                                                                isSecondaryDamage: true);

            yield return base.OnStatusTickInBattle(battler, battleManager, showMessage);
        }

        /// <summary>
        /// Called when calculating the damage of a move.
        /// </summary>
        /// <param name="move">Move being used.</param>
        /// <param name="user">USer of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier to apply to the move.</returns>
        public override float OnCalculateMoveDamage(DamageMove move, Battler user, BattleManager battleManager) => .5f;

        /// <summary>
        /// Play the status animation.
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