using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability MagnetPull.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MagnetPull", fileName = "MagnetPull")]
    public class MagnetPull : Ability
    {
        /// <summary>
        /// Types immune to this ability.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Immunities")]
        private List<MonsterType> ImmuneTypes;

        /// <summary>
        /// Types attracted by this ability.
        /// </summary>
        [SerializeField]
        private List<MonsterType> AttractedTypes;

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
            if (other.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings)
             || !other.IsOfAnyType(AttractedTypes, battleManager.YAPUSettings))
                return true;

            if (showMessages)
                DialogManager.ShowDialog("Abilities/MagnetPull/CantSwitch",
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
            if (other.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings)
             || !other.IsOfAnyType(AttractedTypes, battleManager.YAPUSettings))
                return (true, false);

            if (showMessages)
                DialogManager.ShowDialog("Abilities/MagnetPull/CantSwitch",
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

        /// <summary>
        /// 50% chance of forcing an encounter with an attracted type.
        /// </summary>
        /// <param name="possibleEncounters">Current possible encounters.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="sceneInfo">Info for the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        public override void ModifyPossibleEncounters(ref List<WildEncounter> possibleEncounters,
                                                      MonsterInstance owner,
                                                      SceneInfo sceneInfo,
                                                      EncounterType encounterType)
        {
            if (!(Random.value <= .5f)) return;
            Logger.Info("Attempting to force an encounter with an attracted type.");

            List<WildEncounter> newCandidates = possibleEncounters
                                               .Where(encounter =>
                                                          encounter
                                                             .Monster[encounter.FormCalculator
                                                                         .GetEncounterForm(sceneInfo,
                                                                              encounterType)]
                                                             .IsOfAnyType(AttractedTypes))
                                               .ToList();

            if (newCandidates.Count > 0) possibleEncounters = newCandidates;
        }
    }
}