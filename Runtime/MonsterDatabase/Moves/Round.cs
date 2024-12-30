using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Round.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Round", fileName = "Round")]
    public class Round : FollowUpDamageMove
    {
        /// <summary>
        /// Reference to the FX prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect FXPrefab;

        /// <summary>
        /// Move audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// List of monsters that joined the round this turn.
        /// </summary>
        private readonly List<Battler> joinedRound = new();

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

            VisualEffect fx = Instantiate(FXPrefab, sprite.Pivot);

            AudioManager.Instance.PlayAudio(Audio, pitch: speed);

            yield return new WaitForSeconds(3.2f / speed);

            fx.Stop();

            DOVirtual.DelayedCall(3f, () => Destroy(fx.gameObject));
        }

        /// <summary>
        /// If one of the other moves has been added, follow up.
        /// </summary>
        /// <param name="moveOwner">Owner of the move.</param>
        /// <param name="lastAdded">Last added battler.</param>
        /// <param name="order">Current calculated order.</param>
        /// <param name="battlers">Battlers that will perform actions this turn.</param>
        /// <param name="actions">Actions that will be performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns> An int value.
        /// -1 -> Go immediately before the last action added.
        /// 0 -> Follow normal ordering.
        /// 1 -> Go immediately after the last action added.
        /// </returns>
        public override int OnActionAddedToOrder(Battler moveOwner,
                                                 Battler lastAdded,
                                                 ref Queue<Battler> order,
                                                 List<Battler> battlers,
                                                 SerializableDictionary<Battler, BattleAction> actions,
                                                 BattleManager battleManager)
        {
            int modifier = base.OnActionAddedToOrder(moveOwner, lastAdded, ref order, battlers, actions, battleManager);

            if (modifier == 1) joinedRound.Add(moveOwner);

            return modifier;
        }

        /// <summary>
        /// Execute the effect of the move.
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

            if (joinedRound.Contains(user)) joinedRound.Remove(user);
        }

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            (joinedRound.Contains(user) ? 2 : 1) * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

        /// <summary>
        /// Called when the battle ends to clean up any data the move may have stored.
        /// </summary>
        /// <param name="battler">Battler owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            joinedRound.Clear();

            yield break;
        }
    }
}