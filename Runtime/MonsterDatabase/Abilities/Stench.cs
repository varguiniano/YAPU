using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Stench.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Stench", fileName = "Stench")]
    public class Stench : Ability
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private VolatileStatus Status;

        /// <summary>
        /// Chance to add the status.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float Chance = .1f;

        /// <summary>
        /// Countdown for the status.
        /// -1 = infinite.
        /// </summary>
        [SerializeField]
        private int Countdown = 1;

        /// <summary>
        /// Called after the owner uses a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="owner">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator AfterHittingWithMove(Move move,
                                                         Battler owner,
                                                         List<(BattlerType Type, int Index)> targets,
                                                         BattleManager battleManager)
        {
            if (move is not DamageMove) yield break;

            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers
                                              .GetBattlerFromBattleIndex(targetType,
                                                                         targetIndex);

                if (!target.CanBattle
                 || battleManager.Statuses.HasStatus(Status, targetType, targetIndex)
                 || !battleManager.CurrentTurnActionOrder.Contains(target))
                    yield break;

                float roll = battleManager.RandomProvider.Value01();

                if (!(roll < Chance)) continue;

                ShowAbilityNotification(owner);

                yield return battleManager.Statuses.AddStatus(Status,
                                                              Countdown,
                                                              targetType,
                                                              targetIndex,
                                                              userType,
                                                              userIndex,
                                                              IgnoresOtherAbilities(battleManager, owner, null));
            }
        }

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public override float
            OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType) =>
            0.125f;
    }
}