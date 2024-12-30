using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// ColorChange ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Protean", fileName = "Protean")]
    public class Protean : Ability
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private VolatileStatus Status;

        /// <summary>
        /// Types immune to this ability.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<MonsterType> ImmuneTypes;

        /// <summary>
        /// Items immune to this ability.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Item> ImmuneItems;

        /// <summary>
        /// Should the status be forever?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected bool InfiniteDuration;

        /// <summary>
        /// Useful for moves that need a target but set on the user, like Mind Reader.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool SetStatusOnSelf;

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="move">Move they will use.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Move's targets.</param>
        /// <param name="ignoreStatus">Does the move ignore the battler's status?</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToPerformMove(Move move,
                                                         Battler owner,
                                                         BattleManager battleManager,
                                                         List<(BattlerType Type, int Index)> targets,
                                                         bool ignoreStatus,
                                                         bool ignoresAbilities,
                                                         Action<bool> finished)
        {
            List<Battler> statusTargets = SetStatusOnSelf
                                              ? new List<Battler> {owner}
                                              : targets.Select(target => battleManager.Battlers
                                                                  .GetBattlerFromBattleIndex(target))
                                                       .ToList();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Battler target in statusTargets)
            {
                if (target.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings)) yield break;

                if (target.CanUseHeldItemInBattle(battleManager) && ImmuneItems.Contains(target.HeldItem)) yield break;

                ShowAbilityNotification(owner);

                (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
                (BattlerType targetType, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);

                object[] extraData =
                    PrepareExtraData(Status, move, owner, battleManager, targets, ignoreStatus, ignoresAbilities);

                yield return battleManager.Statuses.AddStatus(Status,
                                                              CalculateCountdown(Status,
                                                                  battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  targetType,
                                                                  targetIndex),
                                                              target,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities,
                                                              extraData);

                yield break;
            }
        }

        /// <summary>
        /// Calculate the countdown 
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        private int CalculateCountdown(VolatileStatus status,
                                       BattleManager battleManager,
                                       BattlerType userType,
                                       int userIndex,
                                       BattlerType targetType,
                                       int targetIndex) =>
            InfiniteDuration
                ? -1
                : status.CalculateRandomCountdown(battleManager, userType, userIndex, targetType, targetIndex);

        /// <summary>
        /// Prepare the extra data for the volatile status.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="ignoreStatus">Does the move ignore statuses?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        private static object[] PrepareExtraData(VolatileStatus status,
                                                 Move move,
                                                 Battler owner,
                                                 BattleManager battleManager,
                                                 List<(BattlerType Type, int Index)> targets,
                                                 bool ignoreStatus,
                                                 bool ignoresAbilities) =>
            new object[] {move.GetMoveTypeInBattle(owner, battleManager), null};
    }
}