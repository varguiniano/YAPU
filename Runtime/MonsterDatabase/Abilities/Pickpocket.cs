using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Pickpocket.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Pickpocket", fileName = "Pickpocket")]
    public class Pickpocket : Ability
    {
        /// <summary>
        /// Items that can't be stolen.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        private List<Item> StealImmuneItems;

        /// <summary>
        /// Abilities that will prevent stealing.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        private List<Ability> StealImmuneAbilities;

        /// <summary>
        /// Items that can't be stolen if either the user or the target are the specified monster.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        private SerializedDictionary<Item, List<MonsterEntry>> ImmuneItemMonsterCombinations;

        /// <summary>
        /// Abilities that prevent stealing depending on the stolen item.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        private SerializedDictionary<Ability, List<Item>> ImmuneItemAbilityCombinations;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="damageDealt">Damage dealt by the move.</param>
        /// <param name="previousHP">HP it had before being hit.</param>
        /// <param name="wasCritical">Was it a critical hit?</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
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
            if (!move.DoesMoveMakeContact(user, owner, battleManager, ignoresAbilities)) yield break;

            // Not the last hit.
            if (hitNumber < expectedMoveHits - 1) yield break;

            yield return StealItem(move,
                                   effectiveness,
                                   owner,
                                   user,
                                   damageDealt,
                                   previousHP,
                                   wasCritical,
                                   substituteTookHit,
                                   ignoresAbilities,
                                   battleManager);
        }

        /// <summary>
        /// Steam the item.
        /// Same code as thief.
        /// </summary>
        private IEnumerator StealItem(DamageMove move,
                                      float effectiveness,
                                      Battler owner,
                                      Battler user,
                                      int damageDealt,
                                      uint previousHP,
                                      bool wasCritical,
                                      bool substituteTookHit,
                                      bool ignoresAbilities,
                                      BattleManager battleManager)
        {
            // Won't work if user fainted.
            if (!owner.CanBattle) yield break;

            // Can't steal if the target doesn't have an item.
            if (user.HeldItem == null) yield break;

            // Can't steal if the user has an item.
            if (owner.HeldItem != null) yield break;

            // Doesn't affect substitutes.
            if (user.Substitute.SubstituteEnabled) yield break;

            if (StealImmuneItems.Contains(user.HeldItem)) yield break;

            if (user.CanUseAbility(battleManager, ignoresAbilities)
             && StealImmuneAbilities.Contains(user.GetAbility()))
            {
                user.GetAbility().ShowAbilityNotification(user);

                yield break;
            }

            foreach (KeyValuePair<Item, List<MonsterEntry>> _ in ImmuneItemMonsterCombinations
                                                                .Where(itemMonsterPair =>
                                                                           user.HeldItem
                                                                        == itemMonsterPair.Key)
                                                                .Where(itemMonsterPair =>
                                                                           itemMonsterPair.Value
                                                                              .Contains(user
                                                                                  .Species)
                                                                        || itemMonsterPair.Value
                                                                              .Contains(owner
                                                                                  .Species)))
                yield break;

            foreach (KeyValuePair<Ability, List<Item>> _ in ImmuneItemAbilityCombinations
                                                           .Where(itemMonsterPair =>
                                                                      user.CanUseAbility(battleManager,
                                                                          ignoresAbilities)
                                                                   && user.GetAbility()
                                                                   == itemMonsterPair.Key)
                                                           .Where(itemMonsterPair =>
                                                                      itemMonsterPair.Value
                                                                         .Contains(user
                                                                             .HeldItem)))
                yield break;

            // Can't steal mega stones from mons that can use them.
            if (user.FormData.MegaEvolutions.ContainsKey(user.HeldItem)) yield break;

            ShowAbilityNotification(owner);

            yield return user.HeldItem.OnItemStolen(user, battleManager);

            Item item = user.HeldItem;

            owner.HeldItem = item;

            user.ConsumedItemData.ConsumedItem = item;
            user.ConsumedItemData.CanBeRecycled = false;
            user.ConsumedItemData.CanBeRecoveredAfterBattle = true;

            user.HeldItem = null;

            yield return owner.HeldItem.OnItemReceivedInBattle(owner, battleManager);

            yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Thief/ItemStolen",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        owner.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        item.GetName(battleManager.Localizer),
                                                                        user.GetNameOrNickName(battleManager.Localizer)
                                                                    });
        }
    }
}