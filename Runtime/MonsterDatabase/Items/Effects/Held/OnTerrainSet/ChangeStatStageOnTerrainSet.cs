using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnTerrainSet
{
    /// <summary>
    /// Data class for an item effect that changes a stat stage when a terrain is set..
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnTerrainSet/ChangeStatStageOnTerrainSet",
                     fileName = "ChangeStatStageOnTerrainSet")]
    public class ChangeStatStageOnTerrainSet : OnTerrainSetItemEffect
    {
        /// <summary>
        /// Terrain to match.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        [SerializeField]
        private Terrain Terrain;

        /// <summary>
        /// Stat to change.
        /// </summary>
        [SerializeField]
        private Stat Stat;

        /// <summary>
        /// Amount to change.
        /// </summary>
        [SerializeField]
        private short Amount;

        /// <summary>
        /// Change even if it will change 0.
        /// </summary>
        [SerializeField]
        private bool ChangeEvenIfWillChangeZero;

        /// <summary>
        /// Consume the item.
        /// </summary>
        [SerializeField]
        private bool Consume;

        /// <summary>
        /// Called when a terrain is set on the battlefield.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="terrain">Terrain that has been set.</param>
        /// <param name="holder">Holder of this item.</param>
        /// <param name="item">Item containing this effect.</param>
        /// <param name="finished">Should the item be consumed?</param>
        public override IEnumerator OnTerrainSet(BattleManager battleManager,
                                                 Terrain terrain,
                                                 Battler holder,
                                                 Item item,
                                                 Action<bool> finished)
        {
            if (Terrain != terrain) yield break;

            (BattlerType holderType, int holderIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(holder);

            // TODO: Contrary.

            short finalAmount = 0;

            item.ShowItemNotification(holder, battleManager.Localizer);

            yield return battleManager.BattlerStats.ChangeStatStage(holderType,
                                                                    holderIndex,
                                                                    Stat,
                                                                    Amount,
                                                                    holderType,
                                                                    holderIndex,
                                                                    showCantChangeMessage: ChangeEvenIfWillChangeZero,
                                                                    finished: modifier => finalAmount = modifier);

            if (Consume)
            {
                if (ChangeEvenIfWillChangeZero)
                    finished.Invoke(true);
                else
                    finished.Invoke(finalAmount != 0);
            }
            else
                finished.Invoke(false);
        }
    }
}