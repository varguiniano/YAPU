using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that set a status to the attacker when hit with a contact move.
    /// </summary>
    public abstract class SetStatusOnContactAbility : Ability
    {
        /// <summary>
        /// Dictionary with the statuses that can be inflicted and the chance to inflict them.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<Status, float> StatusChances;

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
        /// Set the status on the user instead of the attacker.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool SetOnOwner;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="damageDealt"></param>
        /// <param name="previousHP"></param>
        /// <param name="wasCritical"></param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
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
            Battler target = SetOnOwner ? owner : user;

            if (!move.DoesMoveMakeContact(user, owner, battleManager, ignoresAbilities)
             || !AffectsUserOfEffect(target, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager))
                yield break;

            if (target.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings)) yield break;

            if (target.CanUseHeldItemInBattle(battleManager) && ImmuneItems.Contains(target.HeldItem)) yield break;

            foreach (KeyValuePair<Status, float> pair in
                     StatusChances.Where(pair => battleManager.RandomProvider.Value01() <= pair.Value))
            {
                ShowAbilityNotification(owner);

                yield return battleManager.Statuses.AddStatus(pair.Key,
                                                              target,
                                                              owner,
                                                              IgnoresOtherAbilities(battleManager, owner, null));

                yield break;
            }
        }
    }
}