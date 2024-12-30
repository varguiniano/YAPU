using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing a status that modifies typing.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/ReplaceTypeStatus",
                     fileName = "ReplaceTypeStatus")]
    public class ReplaceTypeStatus : VolatileStatus
    {
        /// <summary>
        /// Set the types dynamically?
        /// </summary>
        [SerializeField]
        private bool SetDynamically;

        /// <summary>
        /// Types to remove from the monster.
        /// </summary>
        [HideIf(nameof(SetDynamically))]
        [SerializeField]
        private List<MonsterType> TypesToRemove;

        /// <summary>
        /// Type to default to if there is only one type.
        /// </summary>
        [HideIf(nameof(SetDynamically))]
        [SerializeField]
        private MonsterType DefaultIfSingle;

        /// <summary>
        /// Types for each monster, if set dynamically.
        /// </summary>
        private readonly SerializedDictionary<Battler, (MonsterType, MonsterType)> dynamicTypes = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            // Don't show a message.

            if (SetDynamically) dynamicTypes[battler] = ((MonsterType) extraData[0], (MonsterType) extraData[1]);

            yield break;
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            // Don't show a message.
            yield break;
        }

        /// <summary>
        /// Called when the monster types are being calculated.
        /// </summary>
        /// <param name="battler">Reference to the monster.</param>
        /// <param name="currentCalculatedFirst">First type already calculated.</param>
        /// <param name="currentCalculatedSecond">Second type already calculated.</param>
        /// <returns>The new calculated types.</returns>
        public override (MonsterType, MonsterType) OnCalculateTypes(Battler battler,
                                                                    MonsterType currentCalculatedFirst,
                                                                    MonsterType currentCalculatedSecond)
        {
            if (SetDynamically) return dynamicTypes[battler];

            if ((currentCalculatedSecond == null && TypesToRemove.Contains(currentCalculatedFirst))
             || (TypesToRemove.Contains(currentCalculatedFirst) && TypesToRemove.Contains(currentCalculatedSecond)))
                return (DefaultIfSingle, null);

            if (TypesToRemove.Contains(currentCalculatedFirst)) return (currentCalculatedSecond, null);

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (TypesToRemove.Contains(currentCalculatedSecond)) return (currentCalculatedFirst, null);

            return base.OnCalculateTypes(battler, currentCalculatedFirst, currentCalculatedSecond);
        }
    }
}