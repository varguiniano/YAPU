using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Statuses module for the battle manager.
    /// </summary>
    public class BattleManagerStatusesModule : BattleManagerModule<BattleManagerStatusesModule>
    {
        /// <summary>
        /// Global statuses.
        /// </summary>
        [FoldoutGroup("Global Statuses")]
        [ShowInInspector]
        [ReadOnly]
        private Dictionary<GlobalStatus, int> globalStatuses = new();

        /// <summary>
        /// Side statuses on the ally's side.
        /// </summary>
        [FoldoutGroup("Side Statuses")]
        [ShowInInspector]
        [ReadOnly]
        private Dictionary<SideStatus, int> allySideStatuses = new();

        /// <summary>
        /// Side statuses on the enemy's side.
        /// </summary>
        [FoldoutGroup("Side Statuses")]
        [ShowInInspector]
        [ReadOnly]
        private Dictionary<SideStatus, int> enemySideStatuses = new();

        /// <summary>
        /// Queue of statuses to be added.
        /// </summary>
        private readonly Queue<(VolatileStatus, Battler)> volatileAddQueue = new();

        /// <summary>
        /// Queue of statuses to be removed.
        /// </summary>
        private readonly Queue<(VolatileStatus, Battler)> volatileRemovalQueue = new();

        /// <summary>
        /// Queue for side statuses to be removed.
        /// </summary>
        private readonly Queue<(SideStatus, BattlerType)> sideStatusRemovalQueue = new();

        /// <summary>
        /// Get the global statuses on the field.
        /// </summary>
        public Dictionary<GlobalStatus, int> GetGlobalStatuses() => globalStatuses;

        /// <summary>
        /// Get the side status of a team.
        /// </summary>
        /// <param name="side">Side to get.</param>
        /// <returns>That team's statuses.</returns>
        public Dictionary<SideStatus, int> GetSideStatuses(BattlerType side) =>
            side switch
            {
                BattlerType.Ally => allySideStatuses,
                BattlerType.Enemy => enemySideStatuses,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

        /// <summary>
        /// Set the side statuses of a team.
        /// </summary>
        /// <param name="side">Side to set.</param>
        /// <param name="newStatuses">New statuses.</param>
        private void SetSideStatuses(BattlerType side, Dictionary<SideStatus, int> newStatuses)
        {
            switch (side)
            {
                case BattlerType.Ally:
                    allySideStatuses = newStatuses;
                    break;
                case BattlerType.Enemy:
                    enemySideStatuses = newStatuses;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        /// <summary>
        /// Call callbacks for each battler (even non combatants) when the battle starts.
        /// </summary>
        public IEnumerator OnBattleStartedCallbacks()
        {
            yield return OnBattleStartedCallbacks(BattlerType.Ally);
            yield return OnBattleStartedCallbacks(BattlerType.Enemy);
        }

        /// <summary>
        /// Call callbacks for each battler (even non combatants) when the battle starts.
        /// </summary>
        private IEnumerator OnBattleStartedCallbacks(BattlerType side)
        {
            List<Battler> list = Battlers.GetBattlersFighting(side);

            for (int i = 0; i < list.Count; i++)
            {
                (BattlerType _, int rosterIndex, int _) =
                    Battlers.GetTypeAndRosterIndexOfBattler(list[i]);

                foreach (Battler battler in Rosters.GetRoster(side, rosterIndex))
                    yield return OnBattleStartedCallbacks(battler);
            }
        }

        /// <summary>
        /// Call callbacks for each battler (even non combatants) when the battle starts.
        /// </summary>
        private IEnumerator OnBattleStartedCallbacks(Battler battler)
        {
            if (battler.GetStatus() != null) yield return battler.GetStatus().OnBattleStarted(BattleManager, battler);
        }

        /// <summary>
        /// Trigger the count down of all statuses.
        /// </summary>
        public IEnumerator TriggerStatusesCountdown()
        {
            yield return TriggerStatusRemoval();

            yield return TriggerGlobalStatusesCountdown();

            yield return Scenario.TriggerWeatherCountdown();
            yield return Scenario.TriggerTerrainCountdown();

            yield return TriggerSideStatusesCountdown(BattlerType.Ally);
            yield return TriggerSideStatusesCountdown(BattlerType.Enemy);

            yield return TriggerStatusesCountdown(BattlerType.Ally);
            yield return TriggerStatusesCountdown(BattlerType.Enemy);

            yield return TriggerVolatileStatusesCountdown(BattlerType.Ally);
            yield return TriggerVolatileStatusesCountdown(BattlerType.Enemy);

            // Recheck in case some on tick has removed the status.
            yield return TriggerStatusRemoval();
        }

        /// <summary>
        /// Trigger the ticking of statuses of the battlers on one side.
        /// </summary>
        /// <param name="battlerType">Side.</param>
        private IEnumerator TriggerStatusesCountdown(BattlerType battlerType)
        {
            foreach (Battler battler in Battlers.GetBattlersFighting(battlerType))
            {
                yield return battler.StatusTick(BattleManager);

                yield return BattlerHealth.CheckFaintedBattlers();
            }
        }

        /// <summary>
        /// Trigger volatile statuses countdown for a battler type.
        /// </summary>
        /// <param name="battlerType">Type.</param>
        private IEnumerator TriggerVolatileStatusesCountdown(BattlerType battlerType)
        {
            foreach (Battler battler in Battlers.GetBattlersFighting(battlerType))
            {
                yield return battler.TriggerVolatileStatusesCountdown(BattleManager);

                yield return BattlerHealth.CheckFaintedBattlers();
            }
        }

        /// <summary>
        /// Trigger the countdown of global statuses.
        /// </summary>
        private IEnumerator TriggerGlobalStatusesCountdown()
        {
            Dictionary<GlobalStatus, int> statuses = GetGlobalStatuses();
            Dictionary<GlobalStatus, int> tickedStatuses = new();

            foreach (KeyValuePair<GlobalStatus, int> slot in statuses)
            {
                if (slot.Value == -1)
                {
                    tickedStatuses[slot.Key] = slot.Value;
                    yield return slot.Key.OnTickStatus(BattleManager);
                    continue;
                }

                int newValue = slot.Value - 1;

                if (newValue > 0)
                {
                    tickedStatuses[slot.Key] = newValue;
                    yield return slot.Key.OnTickStatus(BattleManager);
                }
                else
                    yield return slot.Key.OnRemoveStatus(BattleManager);
            }

            globalStatuses = tickedStatuses;

            yield return BattlerHealth.CheckFaintedBattlers();
        }

        /// <summary>
        /// Trigger the countdown of a side's statuses.
        /// </summary>
        /// <param name="side">Side to trigger.</param>
        private IEnumerator TriggerSideStatusesCountdown(BattlerType side)
        {
            Dictionary<SideStatus, int> statuses = GetSideStatuses(side);
            Dictionary<SideStatus, int> tickedStatuses = new();

            foreach (KeyValuePair<SideStatus, int> slot in statuses)
            {
                if (slot.Value == -1)
                {
                    tickedStatuses[slot.Key] = slot.Value;
                    yield return slot.Key.OnTickStatus(BattleManager, side);
                    continue;
                }

                int newValue = slot.Value - 1;

                if (newValue > 0)
                {
                    tickedStatuses[slot.Key] = newValue;
                    yield return slot.Key.OnTickStatus(BattleManager, side);
                }
                else
                {
                    string sideOwner = "";

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (side)
                    {
                        case BattlerType.Ally:
                        case BattlerType.Enemy when BattleManager.EnemyType == EnemyType.Trainer:
                            sideOwner = BattleManager.Localizer[Characters.GetCharacter(side, 0).LocalizableName];
                            break;
                        case BattlerType.Enemy when BattleManager.EnemyType == EnemyType.Wild:
                            sideOwner = Battlers.GetBattlerFromBattleIndex(side, 0)
                                                .GetNameOrNickName(BattleManager.Localizer);

                            break;
                    }

                    yield return slot.Key.EndAnimation(BattleManager, side, sideOwner);
                }
            }

            SetSideStatuses(side, tickedStatuses);

            yield return BattlerHealth.CheckFaintedBattlers();
        }

        /// <summary>
        /// Add a global status.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="user">Monster that added the status.</param>
        /// <param name="countDown">Countdown for the status.</param>
        public IEnumerator AddStatus(GlobalStatus status, Battler user, int countDown)
        {
            yield return status.OnAddStatus(BattleManager, user);

            globalStatuses[status] = countDown;
        }

        /// <summary>
        /// Add a side status to the battle.
        /// </summary>
        /// <param name="statusToAdd">Status to add.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="inBattleIndex">In battle index of the affected roster. Only used for dialogs.</param>
        /// <param name="countDown">Turns it will last. -1 is forever.</param>
        [FoldoutGroup("Side Statuses")]
        [Button("Add Status")]
        private void AddStatusTest(SideStatus statusToAdd,
                                   BattlerType side,
                                   int inBattleIndex,
                                   int countDown) =>
            StartCoroutine(AddStatus(statusToAdd, side, inBattleIndex, countDown, side, inBattleIndex));

        /// <summary>
        /// Add a side status to the battle.
        /// </summary>
        /// <param name="statusToAdd">Status to add.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="inBattleIndex">In battle index of the affected roster. Only used for dialogs.</param>
        /// <param name="countDown">Turns it will last. -1 is forever.</param>
        /// <param name="userType">Type of the battler setting the status.</param>
        /// <param name="userIndex">Index of the battler setting the status.</param>
        /// <param name="extraData">Extra data provided when adding the status.</param>
        public IEnumerator AddStatus(SideStatus statusToAdd,
                                     BattlerType side,
                                     int inBattleIndex,
                                     int countDown,
                                     BattlerType userType,
                                     int userIndex,
                                     params object[] extraData)
        {
            string sideOwner = "";

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (side)
            {
                case BattlerType.Ally:
                case BattlerType.Enemy when BattleManager.EnemyType == EnemyType.Trainer:
                    sideOwner = BattleManager.Localizer[Characters.GetCharacter(side, inBattleIndex)
                                                                  .LocalizableName];

                    break;
                case BattlerType.Enemy when BattleManager.EnemyType == EnemyType.Wild:
                    sideOwner = Battlers.GetBattlerFromBattleIndex(side, inBattleIndex)
                                        .GetNameOrNickName(BattleManager.Localizer);

                    break;
            }

            yield return statusToAdd.StartAnimation(BattleManager, side, sideOwner, extraData);

            int customDuration = BattleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex)
                                              .CalculateSideStatusDuration(statusToAdd,
                                                                           side,
                                                                           inBattleIndex,
                                                                           BattleManager);

            Dictionary<SideStatus, int> statuses = GetSideStatuses(side);

            statuses[statusToAdd] = customDuration != -2 ? customDuration : countDown;
        }

        /// <summary>
        /// Add a status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="battler">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        [FoldoutGroup("Status")]
        [Button("Add Status")]
        private void AddStatusTest(Status status,
                                   BattlerType battler,
                                   int inBattleIndex) =>
            StartCoroutine(AddStatus(status, battler, inBattleIndex, battler, inBattleIndex, false));

        /// <summary>
        /// Add a status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="battler">Battler receiving the status.</param>
        /// <param name="user">Monster that triggered this change.</param>
        /// <param name="ignoreAbilities">Does the adding effect ignore abilities?</param>
        public IEnumerator AddStatus(Status status,
                                     Battler battler,
                                     Battler user,
                                     bool ignoreAbilities)
        {
            (BattlerType targetType, int targetIndex) = Battlers.GetTypeAndIndexOfBattler(battler);
            (BattlerType userType, int userIndex) = Battlers.GetTypeAndIndexOfBattler(user);

            yield return AddStatus(status, targetType, targetIndex, userType, userIndex, ignoreAbilities);
        }

        /// <summary>
        /// Add a status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="type">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="ignoreAbilities">Does the adding effect ignore abilities?</param>
        private IEnumerator AddStatus(Status status,
                                      BattlerType type,
                                      int inBattleIndex,
                                      BattlerType userType,
                                      int userIndex,
                                      bool ignoreAbilities)
        {
            yield return AddStatus(status,
                                   Battlers.GetBattlerFromBattleIndex(type, inBattleIndex),
                                   userType,
                                   userIndex,
                                   ignoreAbilities: ignoreAbilities);
        }

        /// <summary>
        /// Add a status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="battler">Battler to add to.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="takeCurrentStatusIntoAccount">Take the current status into account?</param>
        /// <param name="ignoreAbilities"></param>
        /// <param name="extraData">Extra data for the status.</param>
        public IEnumerator AddStatus(Status status,
                                     Battler battler,
                                     BattlerType userType,
                                     int userIndex,
                                     bool takeCurrentStatusIntoAccount = true,
                                     bool ignoreAbilities = false,
                                     params object[] extraData)
        {
            yield return battler.AddStatusInBattle(status,
                                                   BattleManager,
                                                   userType,
                                                   userIndex,
                                                   ignoreAbilities,
                                                   takeCurrentStatusIntoAccount,
                                                   true,
                                                   extraData: extraData);

            Animation.UpdatePanels();
        }

        /// <summary>
        /// Remove a global status.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        public IEnumerator RemoveStatus(GlobalStatus status)
        {
            if (!globalStatuses.ContainsKey(status)) yield break;

            yield return status.OnRemoveStatus(BattleManager);

            globalStatuses.Remove(status);
        }

        /// <summary>
        /// Remove a status from a battler.
        /// </summary>
        /// <param name="battler">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        [FoldoutGroup("Status")]
        [Button("Remove Status")]
        private void RemoveStatusTest(BattlerType battler,
                                      int inBattleIndex) =>
            StartCoroutine(RemoveStatus(battler, inBattleIndex));

        /// <summary>
        /// Remove a status from a battler.
        /// </summary>
        /// <param name="type">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        private IEnumerator RemoveStatus(BattlerType type,
                                         int inBattleIndex)
        {
            yield return RemoveStatus(Battlers.GetBattlerFromBattleIndex(type, inBattleIndex));
        }

        /// <summary>
        /// Remove a status from a battler.
        /// </summary>
        /// <param name="battler">Battler to add to.</param>
        public IEnumerator RemoveStatus(Battler battler)
        {
            yield return battler.RemoveStatusInBattle(BattleManager);

            Animation.UpdatePanels();
        }

        /// <summary>
        /// Add a volatile status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="countdown">Countdown for this status. -1 = infinite.</param>
        /// <param name="battler">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        [FoldoutGroup("Volatile")]
        [Button("Add Volatile Status")]
        private void AddStatusTest(VolatileStatus status,
                                   int countdown,
                                   BattlerType battler,
                                   int inBattleIndex) =>
            StartCoroutine(AddStatus(status, countdown, battler, inBattleIndex, battler, inBattleIndex, false));

        /// <summary>
        /// Add a volatile status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="countdown">Countdown for this status. -1 = infinite.</param>
        /// <param name="type">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="ignoreAbilities">Does the adding effect ignore abilities?</param>
        /// <param name="extraData">Extra data the status may need.</param>
        public IEnumerator AddStatus(VolatileStatus status,
                                     int countdown,
                                     BattlerType type,
                                     int inBattleIndex,
                                     BattlerType userType,
                                     int userIndex,
                                     bool ignoreAbilities,
                                     params object[] extraData)
        {
            yield return AddStatus(status,
                                   countdown,
                                   Battlers.GetBattlerFromBattleIndex(type, inBattleIndex),
                                   userType,
                                   userIndex,
                                   ignoreAbilities,
                                   extraData);
        }

        /// <summary>
        /// Add a volatile status to a battler.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="countdown">Countdown for this status. -1 = infinite.</param>
        /// <param name="battler">Battler to add to.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="ignoreAbilities">Does the adding effect ignore abilities?</param>
        /// <param name="extraData">Extra data the status may need.</param>
        public IEnumerator AddStatus(VolatileStatus status,
                                     int countdown,
                                     Battler battler,
                                     BattlerType userType,
                                     int userIndex,
                                     bool ignoreAbilities,
                                     params object[] extraData)
        {
            yield return battler.AddVolatileStatus(status,
                                                   countdown,
                                                   BattleManager,
                                                   userType,
                                                   userIndex,
                                                   ignoreAbilities,
                                                   extraData);
        }

        /// <summary>
        /// Does this battle have a global status?
        /// </summary>
        public bool HasStatus(GlobalStatus status) => globalStatuses.ContainsKey(status);

        /// <summary>
        /// Does a side have a status?
        /// </summary>
        /// <param name="status">Status to check.</param>
        /// <param name="type">Battler type.</param>
        public bool HasStatus(SideStatus status, BattlerType type) => GetSideStatuses(type).ContainsKey(status);

        /// <summary>
        /// Does a battler have a status?
        /// </summary>
        /// <param name="status">Status to check.</param>
        /// <param name="type">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        public bool HasStatus(VolatileStatus status, BattlerType type, int inBattleIndex) =>
            HasStatus(status, Battlers.GetBattlerFromBattleIndex(type, inBattleIndex));

        /// <summary>
        /// Does a battler have a status?
        /// </summary>
        /// <param name="status">Status to check.</param>
        /// <param name="battler">Battler to check.</param>
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public bool HasStatus(VolatileStatus status, Battler battler) => battler.HasVolatileStatus(status);

        /// <summary>
        /// Remove a volatile status from a battler.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="battler">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        [FoldoutGroup("Volatile")]
        [Button("Remove Volatile Status")]
        private void RemoveStatusTest(VolatileStatus status, BattlerType battler, int inBattleIndex) =>
            StartCoroutine(RemoveStatus(status, battler, inBattleIndex));

        /// <summary>
        /// Remove a volatile status from a battler.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="type">Battler type.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        public IEnumerator RemoveStatus(VolatileStatus status, BattlerType type, int inBattleIndex)
        {
            yield return RemoveStatus(status, Battlers.GetBattlerFromBattleIndex(type, inBattleIndex));
        }

        /// <summary>
        /// Remove a volatile status from a battler.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="battler">Reference to the battler.</param>
        public IEnumerator RemoveStatus(VolatileStatus status, Battler battler)
        {
            yield return battler.RemoveVolatileStatus(status, BattleManager);
        }

        /// <summary>
        /// Remove a side status from a side.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="side">Side to remove it from.</param>
        public IEnumerator RemoveStatus(SideStatus status, BattlerType side)
        {
            if (!HasStatus(status, side)) yield break;

            string sideOwner;

            switch (side)
            {
                case BattlerType.Ally:
                case BattlerType.Enemy when BattleManager.EnemyType == EnemyType.Trainer:
                    sideOwner = BattleManager.Localizer[Characters.GetCharacter(side, 0).LocalizableName];
                    break;
                case BattlerType.Enemy when BattleManager.EnemyType == EnemyType.Wild:
                    sideOwner = Battlers.GetBattlerFromBattleIndex(side, 0)
                                        .GetNameOrNickName(BattleManager.Localizer);

                    break;
                default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            yield return status.EndAnimation(BattleManager, side, sideOwner);

            switch (side)
            {
                case BattlerType.Ally:
                    allySideStatuses.Remove(status);
                    break;
                case BattlerType.Enemy:
                    enemySideStatuses.Remove(status);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        /// <summary>
        /// Schedule the removal of a status.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="side">side to remove it from.</param>
        public void ScheduleRemoveStatus(SideStatus status, BattlerType side) =>
            sideStatusRemovalQueue.Enqueue((status, side));

        /// <summary>
        /// Schedule the removal of a status.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="battler">Battler to remove it from.</param>
        public void ScheduleAddStatus(VolatileStatus status, Battler battler) =>
            volatileAddQueue.Enqueue((status, battler));

        /// <summary>
        /// Schedule the removal of a status.
        /// </summary>
        /// <param name="status">Status to remove.</param>
        /// <param name="battler">Battler to remove it from.</param>
        public void ScheduleRemoveStatus(VolatileStatus status, Battler battler) =>
            volatileRemovalQueue.Enqueue((status, battler));

        /// <summary>
        /// Remove statuses which removal was pending.
        /// </summary>
        public IEnumerator TriggerStatusRemoval()
        {
            while (sideStatusRemovalQueue.TryDequeue(out (SideStatus status, BattlerType side) tuple))
                yield return RemoveStatus(tuple.status, tuple.side);

            while (volatileRemovalQueue.TryDequeue(out (VolatileStatus, Battler) tuple))
                yield return RemoveStatus(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// Callback for when a battler enters the battle.
        /// </summary>
        /// <param name="side">Side of this status.</param>
        /// <param name="battlerIndex">Index of the battler.</param>
        public IEnumerator OnBattlerEnteredSide(BattlerType side, int battlerIndex) =>
            GetSideStatuses(side)
               .Select(sideStatusSlot => sideStatusSlot.Key.OnBattlerEnteredSide(side, battlerIndex, BattleManager))
               .GetEnumerator();

        /// <summary>
        /// Called when the battle has ended.
        /// </summary>
        public IEnumerator OnBattleEnded()
        {
            foreach (GlobalStatus status in globalStatuses.Select(slot => slot.Key))
                yield return status.OnBattleEnded(BattleManager);

            yield return OnBattleEnded(BattlerType.Ally);
            yield return OnBattleEnded(BattlerType.Enemy);
        }

        /// <summary>
        /// Called when the battle has ended.
        /// </summary>
        /// <param name="side">Side the status is setup in.</param>
        private IEnumerator OnBattleEnded(BattlerType side) =>
            GetSideStatuses(side).Select(statusSlot => statusSlot.Key.OnBattleEnded(side)).GetEnumerator();
    }
}