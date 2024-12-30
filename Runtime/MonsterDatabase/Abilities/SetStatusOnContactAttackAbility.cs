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
    /// Very similar to the SetStatusOnContactAbility, but this one sets the status on when attacking instead of being attacked.
    /// </summary>
    public abstract class SetStatusOnContactAttackAbility : Ability
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
        /// Check the contact and inflict the status.
        /// </summary>
        public override IEnumerator AfterUsingMove(Move move,
                                                   Battler owner,
                                                   List<(BattlerType Type, int Index)> targets,
                                                   BattleManager battleManager)
        {
            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                if (!move.DoesMoveMakeContact(owner,
                                              target,
                                              battleManager,
                                              IgnoresOtherAbilities(battleManager, owner, move))
                 || !AffectsUserOfEffect(owner,
                                         target,
                                         IgnoresOtherAbilities(battleManager, owner, move),
                                         battleManager))
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
}