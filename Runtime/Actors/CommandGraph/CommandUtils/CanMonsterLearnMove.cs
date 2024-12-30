using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Scriptable object that can check the compatibility of a monster on a choosing dialog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dialogs/MonsterCompatibility/CanMonsterLearnMove",
                     fileName = "CanMonsterLearnMove")]
    public class CanMonsterLearnMove : MonsterCompatibilityChecker
    {
        /// <summary>
        /// Move to check.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private MonsterDatabase.Moves.Move Move;

        /// <summary>
        /// Localization key to use when the monster is not compatible.
        /// </summary>
        public override string GetNotCompatibleLocalizationKey(MonsterInstance monster) =>
            monster.KnowsMove(Move)
                ? "Dialogs/MonsterCompatibility/AlreadyKnows"
                : "Dialogs/MonsterCompatibility/CantLearn";

        /// <summary>
        /// Check if a monster is compatible.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="settings"></param>
        /// <returns>True if it is compatible.</returns>
        public override bool IsMonsterCompatible(MonsterInstance monster, YAPUSettings settings) =>
            !monster.KnowsMove(Move) && monster.CanLearnMove(Move);
    }
}