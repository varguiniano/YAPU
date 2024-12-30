using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Ability that sets up a terrain when entering the battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SetupTerrainWhenEnteringBattle",
                     fileName = "SetupTerrainWhenEnteringBattle")]
    public class SetupTerrainWhenEnteringBattle : Ability
    {
        /// <summary>
        /// Reference to the terrain the ability will set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Terrain TerrainToSet;

        /// <summary>
        /// Countdown for the terrain to last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int Countdown;

        /// <summary>
        /// Reference to the terrain that are incompatible if set and will make the ability fail.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Terrain> IncompatibleTerrains;

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            if (IncompatibleTerrains.Contains(battleManager.Scenario.Terrain)) yield break;

            int customCountdown = battler.CanUseHeldItemInBattle(battleManager)
                                      ? battler.HeldItem.CalculateTerrainDuration(battler, TerrainToSet, battleManager)
                                      : -2;

            ShowAbilityNotification(battler);

            yield return
                battleManager.Scenario.SetTerrain(TerrainToSet, customCountdown != -2 ? customCountdown : Countdown);
        }
    }
}