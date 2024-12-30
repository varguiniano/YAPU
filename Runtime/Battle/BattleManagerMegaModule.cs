using System.Collections;
using Sirenix.OdinInspector;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Module of the battle manager that handles mega forms.
    /// </summary>
    public class BattleManagerMegaModule : BattleManagerModule<BattleManagerMegaModule>
    {
        /// <summary>
        /// Dictionary storing the rosters that have megaevolved.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private SerializableDictionary<(BattlerType, int), bool> rostersThatMegaevolved = new();

        /// <summary>
        /// Check if a monster can megaevolve.
        /// </summary>
        /// <param name="battlerType">Battler type of the roster to check.</param>
        /// <param name="inBattleIndex">In battle index of the roster to check.</param>
        /// <returns>True if it can.</returns>
        public bool CanMegaevolve(BattlerType battlerType, int inBattleIndex) =>
            CanMegaevolve(Battlers.GetBattlerFromBattleIndex(battlerType, inBattleIndex));

        /// <summary>
        /// Check if a monster can megaevolve.
        /// </summary>
        /// <param name="battler">Monster to check.</param>
        /// <returns>True if it can.</returns>
        public bool CanMegaevolve(Battler battler) =>
            battler.CanSwitchToMegaForm(BattleManager) && !HasRosterMegaevolved(battler);

        /// <summary>
        /// Check the roster of a battler has already megaevolved this battle.
        /// </summary>
        /// <param name="battler">Battler to check the roster from.</param>
        /// <returns>True if it already has megaevolved this battle.</returns>
        private bool HasRosterMegaevolved(Battler battler) =>
            HasRosterMegaevolved(Battlers.GetTypeAndIndexOfBattler(battler));

        /// <summary>
        /// Check if a roster has already megaevolved this battle.
        /// </summary>
        /// <param name="typeIndexTuple">Battler type of the roster to check and In battle index of the roster to check.</param>
        /// <returns>True if it already has megaevolved this battle.</returns>
        private bool HasRosterMegaevolved((BattlerType, int) typeIndexTuple) =>
            HasRosterMegaevolved(typeIndexTuple.Item1, typeIndexTuple.Item2);

        /// <summary>
        /// Check if a roster has already megaevolved this battle.
        /// </summary>
        /// <param name="battlerType">Battler type of the roster to check.</param>
        /// <param name="inBattleIndex">In battle index of the roster to check.</param>
        /// <returns>True if it already has megaevolved this battle.</returns>
        private bool HasRosterMegaevolved(BattlerType battlerType, int inBattleIndex)
        {
            (int rosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(battlerType, inBattleIndex);

            return rostersThatMegaevolved.ContainsKey((battlerType, rosterIndex))
                && rostersThatMegaevolved[(battlerType, rosterIndex)];
        }

        /// <summary>
        /// Trigger the megaevolution of a battler.
        /// </summary>
        /// <param name="battler">Battler to mega evolve.</param>
        public IEnumerator TriggerMegaevolution(Battler battler)
        {
            (BattlerType battlerType, int battlerIndex) = Battlers.GetTypeAndIndexOfBattler(battler);

            (int rosterIndex, int _) = Rosters.InBattleIndexToRosterIndex(battlerType, battlerIndex);
            rostersThatMegaevolved[(battlerType, rosterIndex)] = true;

            yield return Battlers.ChangeForm(battlerType,
                                             battlerIndex,
                                             battler.GetAvailableMegaForm(BattleManager),
                                             true,
                                             "Dialogs/Battle/Megaevolved");

            // Ability ETBs are retriggered when the monster megaevolves.
            if (battler.CanUseAbility(BattleManager, false))
                yield return battler.GetAbility().OnMonsterEnteredBattle(BattleManager, battler);
        }
    }
}