using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// CurseBody ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/CurseBody", fileName = "CurseBody")]
    public class CurseBody : Ability
    {
        /// <summary>
        /// Reference to the disable status.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private VolatileStatus DisableStatus;

        /// <summary>
        /// Chance to disable the move.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float DisableChance = 0.3f;

        /// <summary>
        /// Disable the move that hit it.
        /// </summary>
        public override IEnumerator AfterHitByMove(DamageMove move,
                                                   float effectiveness,
                                                   Battler owner,
                                                   Battler user,
                                                   int damageDealt,
                                                   uint previousHP,
                                                   bool wasCritical,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   int hitNumber,
                                                   int expectedMoveHits,
                                                   BattleManager battleManager)
        {
            if (battleManager.RandomProvider.Value01() > DisableChance) yield break;

            (BattlerType ownerType, int ownerIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);

            ShowAbilityNotification(owner);

            yield return battleManager.Statuses.AddStatus(DisableStatus, 4, user, ownerType, ownerIndex, false, move);
        }
    }
}