using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability OwnTempo.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/OwnTempo", fileName = "OwnTempo")]
    public class OwnTempo : Ability
    {
        /// <summary>
        /// Effects that are cured after each action.
        /// This helps removing effects that the monster had before gaining the ability.
        /// </summary>
        [SerializeField]
        private List<VolatileStatus> AfterActionCuredEffects;

        /// <summary>
        /// Remove effects that the monster had before gaining the ability.
        /// </summary>
        public override IEnumerator AfterAction(Battler owner,
                                                BattleAction action,
                                                Battler user,
                                                BattleManager battleManager)
        {
            foreach (VolatileStatus status in AfterActionCuredEffects.Where(owner.HasVolatileStatus))
            {
                ShowAbilityNotification(owner);

                (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);

                yield return battleManager.Statuses.RemoveStatus(status, type, index);
            }
        }
    }
}