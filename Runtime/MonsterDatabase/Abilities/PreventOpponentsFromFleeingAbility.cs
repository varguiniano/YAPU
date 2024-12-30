using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that prevent opponents from fleeing.
    /// </summary>
    public abstract class PreventOpponentsFromFleeingAbility : Ability
    {
        /// <summary>
        /// Types immune to this ability.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Immunities")]
        private List<MonsterType> ImmuneTypes;

        /// <summary>
        /// Dialog to use when the opponent can't switch.
        /// </summary>
        [SerializeField]
        private string CantSwitchDialogKey;

        /// <summary>
        /// Dialog to use when the opponent can't run.
        /// </summary>
        [SerializeField]
        private string CantRunDialog;

        /// <summary>
        /// Check if the battler can switch.
        /// </summary>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="other">The battler trying to run away.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the battler that wants to force switching.</param>
        /// <param name="userIndex">Index of the battler that wants to force switching.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>True if it can.</returns>
        public override bool CanOpponentSwitch(Battler owner,
                                               Battler other,
                                               BattleManager battleManager,
                                               BattlerType userType,
                                               int userIndex,
                                               Move userMove,
                                               Item item,
                                               bool itemBelongsToUser,
                                               bool showMessages)
        {
            if (other.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings) || !other.IsGrounded(battleManager, false))
                return true;

            if (showMessages)
                DialogManager.ShowDialog(CantSwitchDialogKey,
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[]
                                                    {
                                                        battleManager.Localizer[LocalizableName],
                                                        owner.GetNameOrNickName(battleManager.Localizer),
                                                        other.GetNameOrNickName(battleManager.Localizer)
                                                    },
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return false;
        }

        /// <summary>
        /// Check if the battler can run away.
        /// </summary>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="other">The battler trying to run away.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessages">Should show explanation messages?</param>
        /// <returns>If it can run away, and if it overrides all other effects preventing run away.</returns>
        public override (bool, bool) CanOpponentMonsterRunAway(Battler owner,
                                                               Battler other,
                                                               BattleManager battleManager,
                                                               bool showMessages)
        {
            if (other.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings) || !other.IsGrounded(battleManager, false))
                return (true, false);

            if (showMessages)
                DialogManager.ShowDialog(CantRunDialog,
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[]
                                                    {
                                                        battleManager.Localizer[LocalizableName],
                                                        owner.GetNameOrNickName(battleManager.Localizer),
                                                        other.GetNameOrNickName(battleManager.Localizer)
                                                    },
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            return (false, false);
        }
    }
}