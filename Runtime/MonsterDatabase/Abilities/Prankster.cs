using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Prankster ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Prankster", fileName = "Prankster")]
    public class Prankster : Ability
    {
        /// <summary>
        /// Types immune to this ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [SerializeField]
        [FoldoutGroup("Immunities")]
        private List<MonsterType> ImmuneTypes;

        /// <summary>
        /// +1 to priority 0 status moves.
        /// </summary>
        /// <param name="move">Move.</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="currentPriority">Priority of the move before modifications.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications for abilities or items that modify the priority.</param>
        /// <returns>Modifier to apply to the priority.</returns>
        public override int GetMovePriorityModifier(Move move,
                                                    Battler owner,
                                                    List<Battler> targets,
                                                    int currentPriority,
                                                    BattleManager battleManager,
                                                    bool showNotifications)
        {
            if (targets.Count == 0
             || move.GetMoveCategory(owner, targets[0], false, battleManager) != Move.Category.Status
             || currentPriority != 0
             || targets.Any(target => target.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings)))
                return 0;

            if (showNotifications) ShowAbilityNotification(owner);
            return 1;
        }
    }
}