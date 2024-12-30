using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move curse when used by ghosts.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/GhostTypeCurse", fileName = "GhostTypeCurse")]
    public class GhostTypeCurse : SetVolatileStatusMove
    {
        /// <summary>
        /// HP reduction of the curse.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPReduction = .5f;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Reference to the cursed audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// The curse target is selected randomly.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User type.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="targets">Current preselected targets.</param>
        internal override List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
            BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> targets)
        {
            targets.Clear();

            targets.Add(battleManager.Battlers.GetTypeAndIndexOfBattler(battleManager.RandomProvider
                                                                           .RandomElement(battleManager.Battlers
                                                                               .GetBattlersFighting(userType
                                                                                     == BattlerType
                                                                                           .Ally
                                                                                        ? BattlerType
                                                                                           .Enemy
                                                                                        : BattlerType
                                                                                           .Ally))));

            return targets;
        }

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

            BasicSpriteAnimation animation = Instantiate(AnimationPrefab, sprite.Pivot);

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return animation.PlayAnimation(battleManager.BattleSpeed);

            Destroy(animation.gameObject);
        }

        /// <summary>
        /// Set a volatile status on the targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            yield return battleManager.BattlerHealth.ChangeLife(user,
                                                                userType,
                                                                userIndex,
                                                                -Mathf.Max((int) (MonsterMathHelper.CalculateStat(user,
                                                                                           Stat.Hp,
                                                                                           battleManager)
                                                                                     * HPReduction),
                                                                           1),
                                                                playAudio: false);

            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);
        }
    }
}