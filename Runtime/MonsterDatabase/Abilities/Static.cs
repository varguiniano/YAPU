using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Static ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Static", fileName = "Static")]
    public class Static : SetStatusOnContactAbility
    {
        /// <summary>
        /// Reference to the electric type.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType ElectricType;

        /// <summary>
        /// 50% chance of forcing an encounter with an electric type.
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
            Logger.Info("Attempting to force an encounter with an electric type.");

            List<WildEncounter> newCandidates = possibleEncounters
                                               .Where(encounter =>
                                                          encounter
                                                             .Monster[encounter.FormCalculator
                                                                         .GetEncounterForm(sceneInfo,
                                                                              encounterType)]
                                                             .IsOfType(ElectricType))
                                               .ToList();

            if (newCandidates.Count > 0) possibleEncounters = newCandidates;
        }
    }
}