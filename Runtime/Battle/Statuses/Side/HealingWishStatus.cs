using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Base class for the Healing Wish side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/HealingWishStatus", fileName = "HealingWishStatus")]
    public class HealingWishStatus : SideStatus
    {
        /// <summary>
        /// List containing the positions that made a wish.
        /// </summary>
        private List<(BattlerType battlerType, int battlerIndex)> wishes = new();

        /// <summary>
        /// Play an animation when this status starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="sideOwner">Used for dialogs.</param>
        /// <param name="extraData">Extra data provided when adding the status.</param>
        public override IEnumerator StartAnimation(BattleManager battleManager,
                                                   BattlerType side,
                                                   string sideOwner,
                                                   params object[] extraData)
        {
            // No message.

            wishes.Add(((BattlerType) extraData[0], (int) extraData[1]));

            yield break;
        }

        /// <summary>
        /// Play an animation when this status ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        /// <param name="sideOwner">Owner of the side, used for dialogs.</param>
        public override IEnumerator EndAnimation(BattleManager battleManager, BattlerType side, string sideOwner)
        {
            // No dialog.

            // Remove the wishes from a side if the status on that side is removed.
            wishes = wishes.Where(wishData => wishData.battlerType != side).ToList();

            yield break;
        }

        /// <summary>
        /// Trigger the wish when a battler enters one of the wished positions.
        /// </summary>
        public override IEnumerator OnBattlerEnteredSide(BattlerType battlerType,
                                                         int battlerIndex,
                                                         BattleManager battleManager)
        {
            yield return base.OnBattlerEnteredSide(battlerType, battlerIndex, battleManager);

            if (!wishes.Contains((battlerType, battlerIndex))) yield break;

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(battlerType, battlerIndex);

            if (battler.CanHeal(battleManager))
            {
                yield return battleManager.GetMonsterSprite(battlerType, battlerIndex)
                                          .FXAnimator.PlayBoostRoutine(battleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait("Moves/HealingWish/Effect",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                    battler,
                                                                    (int) battler.GetStats(battleManager)[Stat.Hp]);

                yield return battleManager.Statuses.RemoveStatus(battler);
            }

            CheckRemainingWishes(battleManager);
        }

        /// <summary>
        /// Check the remaining wishes and remove unnecessary ones.
        /// </summary>
        private void CheckRemainingWishes(BattleManager battleManager)
        {
            if (wishes.Count(slot => slot.battlerType == BattlerType.Ally) == 0)
                battleManager.Statuses.ScheduleRemoveStatus(this, BattlerType.Ally);

            if (wishes.Count(slot => slot.battlerType == BattlerType.Enemy) == 0)
                battleManager.Statuses.ScheduleRemoveStatus(this, BattlerType.Enemy);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="side">Side the status is setup in.</param>
        public override IEnumerator OnBattleEnded(BattlerType side)
        {
            yield return base.OnBattleEnded(side);

            wishes.Clear();
        }
    }
}