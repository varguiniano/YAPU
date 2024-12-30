using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Base class for the Wish side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/Wish", fileName = "WishSideStatus")]
    public class WishStatus : SideStatus
    {
        /// <summary>
        /// Percentage the wish will heal.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float PercentageToHeal = .5f;

        /// <summary>
        /// Turns until healing.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int TurnsUntilHeal = 2;

        /// <summary>
        /// Dictionary containing the positions that made a wish, the HP they will be healed and the turns left to heal.
        /// </summary>
        private Dictionary<(BattlerType Type, int Index), (int HP, int turns)> wishes = new();

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

            (BattlerType Type, int Index) battlerData = ((BattlerType) extraData[0], (int) extraData[1]);

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(battlerData);

            int hpToRegen = (int) (MonsterMathHelper.CalculateStat(battler,
                                                                   Stat.Hp,
                                                                   battleManager)
                                 * PercentageToHeal);

            wishes[battlerData] = (hpToRegen, TurnsUntilHeal);

            yield break;
        }

        /// <summary>
        /// Callback for when this status is tick each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side it's in.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, BattlerType side)
        {
            yield return base.OnTickStatus(battleManager, side);

            Dictionary<(BattlerType Type, int Index), (int HP, int turns)> remainingWishes = new();

            foreach (KeyValuePair<(BattlerType, int), (int, int)> wish in wishes)
            {
                (BattlerType Type, int Index) battlerData = wish.Key;
                (int hp, int turns) = wish.Value;

                turns--;

                if (turns > 0)
                    remainingWishes[battlerData] = (hp, turns);
                else
                    yield return TriggerWish(battlerData.Type, battlerData.Index, hp, battleManager);
            }

            wishes = remainingWishes;

            CheckRemainingWishes(battleManager);
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

            Dictionary<(BattlerType Type, int Index), (int HP, int turns)> filteredWishes = new();

            // Remove the wishes from a side if the status on that side is removed.
            foreach (((BattlerType battlerType, int battlerIndex), (int, int) wishData) in wishes)
                if (battlerType != side)
                    filteredWishes[(battlerType, battlerIndex)] = wishData;

            wishes = filteredWishes;

            yield break;
        }

        /// <summary>
        /// Called just before checking if a battler is fainted.
        /// </summary>
        /// <param name="side">Side this status is on.</param>
        /// <param name="battlerIndex">Index of the battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns></returns>
        public override IEnumerator OnCheckFaintedBattler(BattlerType side,
                                                          int battlerIndex,
                                                          BattleManager battleManager)
        {
            yield return base.OnCheckFaintedBattler(side, battlerIndex, battleManager);

            if (wishes.ContainsKey((side, battlerIndex))
             && battleManager.Battlers.GetBattlerFromBattleIndex(side, battlerIndex).CurrentHP <= 0)
            {
                yield return TriggerWish(side, battlerIndex, wishes[(side, battlerIndex)].HP, battleManager);
                wishes.Remove((side, battlerIndex));
            }

            CheckRemainingWishes(battleManager);
        }

        /// <summary>
        /// Trigger a wish effect and heal..
        /// </summary>
        private static IEnumerator TriggerWish(BattlerType battlerType,
                                               int battlerIndex,
                                               int hp,
                                               BattleManager battleManager)
        {
            Battler battler = battleManager.Battlers
                                           .GetBattlerFromBattleIndex(battlerType,
                                                                      battlerIndex);

            if (!battler.CanHeal(battleManager)) yield break;

            yield return battleManager.GetMonsterSprite(battlerType, battlerIndex)
                                      .FXAnimator.PlayBoostRoutine(battleManager.BattleSpeed);

            int amount = 0;

            yield return battleManager.BattlerHealth.ChangeLife(battlerType,
                                                                battlerIndex,
                                                                battlerType,
                                                                battlerIndex,
                                                                hp,
                                                                finished: (regenerated, _) =>
                                                                              amount = regenerated);

            yield return DialogManager.ShowDialogAndWait("Status/Side/WishSideStatus/Heal",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        amount.ToString()
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f
                                                           / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Check the remaining wishes and remove unnecessary ones.
        /// </summary>
        private void CheckRemainingWishes(BattleManager battleManager)
        {
            if (wishes.Count(slot => slot.Key.Type == BattlerType.Ally) == 0)
                battleManager.Statuses.ScheduleRemoveStatus(this, BattlerType.Ally);

            if (wishes.Count(slot => slot.Key.Type == BattlerType.Enemy) == 0)
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