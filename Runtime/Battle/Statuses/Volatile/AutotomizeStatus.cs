using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the AutotomizeStatus status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/AutotomizeStatus",
                     fileName = "AutotomizeStatus")]
    public class AutotomizeStatus : VolatileStatus
    {
        /// <summary>
        /// Dictionary that keeps track of how many times a battler has used Autotomize.
        /// </summary>
        private readonly Dictionary<Battler, int> autotomizeLevels = new();

        /// <summary>
        /// Each time the status is added, add one to the level.
        /// </summary>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield return base.OnAddStatus(battleManager, battler, extraData);

            if (!autotomizeLevels.TryAdd(battler, 1)) autotomizeLevels[battler]++;
        }

        /// <summary>
        /// Remove from the dict but not messages.
        /// </summary>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            autotomizeLevels.Remove(battler);
            yield break;
        }

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            yield return base.OnBattleEnded(battler);

            autotomizeLevels.Clear();
        }

        /// <summary>
        /// Get the monster weight in battle.
        /// </summary>
        /// <param name="battler">The owner of the status.</param>
        /// <returns>The modifier to apply to the weight and a full override of the weight.</returns>
        public override (float, float) GetMonsterWeightInBattle(Battler battler) =>
            (autotomizeLevels.TryGetValue(battler, out int level) ? level * -100 : 0, 0);
    }
}