using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.InBattle
{
    /// <summary>
    /// Item effect that adds a side battle status of the side of the target.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/InBattle/AddSideStatusInBattleEffect",
                     fileName = "AddSideStatusInBattleEffect")]
    public class AddSideStatusInBattleEffect : UseInBattleItemEffect
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllSideStatuses))]
        #endif
        [SerializeField]
        private SideStatus Status;

        /// <summary>
        /// The turns the status will last.
        /// </summary>
        [SerializeField]
        private int Turns;

        /// <summary>
        /// Check if the item can be used in this moment.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="user">User of the item.</param>
        /// <returns></returns>
        public override bool CanBeUsed(BattleManager battleManager, Battler user)
        {
            (BattlerType battlerType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            return base.CanBeUsed(battleManager, user)
                && battleManager.Statuses.GetSideStatuses(battlerType).All(pair => pair.Key != Status);
        }

        /// <summary>
        /// Use in battle.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="userType">Battler type of the user.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="battleManager"></param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator Use(Item item,
                                        BattlerType userType,
                                        int userIndex,
                                        BattleManager battleManager,
                                        Action<bool> finished)
        {
            Logger.Info("Used " + Status + " add item by " + userIndex + ", index " + userIndex);

            yield return battleManager.Statuses.AddStatus(Status, userType, userIndex, Turns, userType, userIndex);

            finished?.Invoke(true);
        }
    }
}