using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// BadlyPoison monster status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/BadlyPoison", fileName = "BadlyPoison")]
    public class BadlyPoison : Poison
    {
        /// <summary>
        /// Dictionary that stores the counter of bad poison per battler.
        /// </summary>
        private readonly Dictionary<Battler, byte> countersPerBattler = new();

        /// <summary>
        /// Bad poison can never be added outside of battle.
        /// </summary>
        public override bool CanAddStatus(MonsterInstance monsterInstance,
                                          YAPUSettings settings,
                                          MonsterInstance user = null,
                                          bool takeCurrentStatusIntoAccount = true) =>
            false;

        /// <summary>
        /// Bad poison doesn't take into account if it could be added out of battle since it never can.
        /// Instead uses the base check in battle.
        /// </summary>
        /// <param name="battler">Reference to that monster.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">Monster that added the effect.</param>
        /// <param name="ignoresAbilities">Does the adding effect ignore abilities?</param>
        /// <param name="takeCurrentStatusIntoAccount">Take the current status into account?</param>
        /// <returns>True if it can be added.</returns>
        public override bool
            CanAddStatus(Battler battler,
                         BattleManager battleManager,
                         Battler user,
                         bool ignoresAbilities,
                         bool takeCurrentStatusIntoAccount = true) =>
            (!battleManager.Scenario.GetWeather(out Weather weather) || !ImmuneWeathers.Contains(weather))
         && !ImmuneTerrains.Contains(battleManager.Scenario.Terrain)
         && (!battler.CanUseHeldItemInBattle(battleManager) || !ImmuneHeldItems.Contains(battler.HeldItem))
         && (!battler.CanUseAbility(battleManager, ignoresAbilities)
          || !ImmuneAbilities.Contains(battler.GetAbility())
          || !battler.GetAbility()
                     .AffectsUserOfEffect(user,
                                          battler,
                                          ignoresAbilities,
                                          battleManager))
         && base.CanAddStatus(battler,
                              battleManager.YAPUSettings,
                              user,
                              takeCurrentStatusIntoAccount);

        /// <summary>
        /// Called when the status is added in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        /// <param name="extraData">Not used.</param>
        public override IEnumerator OnStatusAddedInBattle(Battler battler,
                                                          BattleManager battleManager,
                                                          bool ignoresAbilities,
                                                          bool showMessage = true,
                                                          params object[] extraData)
        {
            countersPerBattler[battler] = 0;

            return base.OnStatusAddedInBattle(battler, battleManager, ignoresAbilities, showMessage);
        }

        /// <summary>
        /// Called when the status is ticked in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusTickInBattle(Battler battler,
                                                         BattleManager battleManager,
                                                         bool showMessage = true)
        {
            if (countersPerBattler[battler] < 16) countersPerBattler[battler]++;

            return base.OnStatusTickInBattle(battler, battleManager, showMessage);
        }

        /// <summary>
        /// Calculate the poison damage each tick.
        /// </summary>
        /// <param name="battler">The battler infected.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>The value of the damage to take.</returns>
        protected override int CalculatePoisonDamage(Battler battler, BattleManager battleManager) =>
            base.CalculatePoisonDamage(battler, battleManager) * countersPerBattler[battler];

        /// <summary>
        /// Called when the status is removed out of battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusRemovedInBattle(Battler battler,
                                                            BattleManager battleManager,
                                                            bool showMessage = true)
        {
            countersPerBattler.Remove(battler);
            return base.OnStatusRemovedInBattle(battler, battleManager, showMessage);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            countersPerBattler.Remove(battler);
            return base.OnBattleEnded(battler);
        }
    }
}