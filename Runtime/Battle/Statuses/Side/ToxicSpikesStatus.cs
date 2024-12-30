using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Data class for the Toxic Spikes side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/ToxicSpikes", fileName = "ToxicSpikesStatus")]
    public class ToxicSpikesStatus : LayeredSideStatus
    {
        /// <summary>
        /// Reference to the poison status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        private Status Poison;

        /// <summary>
        /// Reference to the bad poison status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        private Status BadPoison;

        /// <summary>
        /// Reference to the poison type.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType PoisonType;

        /// <summary>
        /// Callback for when a battler enters the battle on the side this status is in.
        /// </summary>
        /// <param name="side">Side of this status.</param>
        /// <param name="battlerIndex">Index of the battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator OnBattlerEnteredSide(BattlerType side,
                                                         int battlerIndex,
                                                         BattleManager battleManager)
        {
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(side, battlerIndex);

            if (!battler.IsGrounded(battleManager, false)) yield break;

            // Poison types remove the toxic spikes.
            if (battler.IsOfType(PoisonType, battleManager.YAPUSettings))
            {
                battleManager.Statuses.ScheduleRemoveStatus(this, side);
                LayerCount.Remove(side);
            }
            else // 2 or more layers means bad poison.
                yield return battleManager.Statuses.AddStatus(LayerCount[side] >= 2 ? BadPoison : Poison,
                                                              battler,
                                                              battler,
                                                              false);
        }
    }
}