using Varguiniano.YAPU.Runtime.Battle.Random;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Base class for modules of the battle manager.
    /// </summary>
    /// <typeparam name="T">The own class.</typeparam>
    public abstract class BattleManagerModule<T> : WhateverBehaviour<T> where T : BattleManagerModule<T>
    {
        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        protected BattleManager BattleManager => GetCachedComponent<BattleManager>();

        /// <summary>
        /// Battlers module.
        /// </summary>
        protected BattleManagerBattlersModule Battlers => BattleManager.Battlers;

        /// <summary>
        /// Battler switch module.
        /// </summary>
        protected BattleManagerBattlerSwitchModule BattleManagerBattlerSwitch =>
            BattleManager.BattleManagerBattlerSwitch;

        /// <summary>
        /// Battler stats module.
        /// </summary>
        protected BattleManagerBattlerStatsModule BattlerStats => BattleManager.BattlerStats;

        /// <summary>
        /// Battler health module.
        /// </summary>
        protected BattleManagerHealthModule BattlerHealth => BattleManager.BattlerHealth;

        /// <summary>
        /// Moves module.
        /// </summary>
        protected BattleManagerMovesModule Moves => BattleManager.Moves;

        /// <summary>
        /// Items module.
        /// </summary>
        protected BattleManagerItemsModule Items => BattleManager.Items;

        /// <summary>
        /// Capture module.
        /// </summary>
        protected BattleManagerCaptureModule Capture => BattleManager.Capture;

        /// <summary>
        /// Rosters module.
        /// </summary>
        protected BattleManagerRostersModule Rosters => BattleManager.Rosters;

        /// <summary>
        /// AI module.
        /// </summary>
        protected BattleManagerAIModule AI => BattleManager.AI;

        /// <summary>
        /// Scenarios module.
        /// </summary>
        protected BattleManagerScenariosModule Scenario => BattleManager.Scenario;

        /// <summary>
        /// Statuses module.
        /// </summary>
        protected BattleManagerStatusesModule Statuses => BattleManager.Statuses;

        /// <summary>
        /// Characters module.
        /// </summary>
        protected BattleManagerCharactersModule Characters => BattleManager.Characters;

        /// <summary>
        /// Animation module.
        /// </summary>
        protected BattleManagerAnimationModule Animation => BattleManager.Animation;

        /// <summary>
        /// Audio module.
        /// </summary>
        protected BattleManagerAudioModule Audio => BattleManager.Audio;

        /// <summary>
        /// Mega forms module.
        /// </summary>
        protected BattleManagerMegaModule Megas => BattleManager.Megas;

        /// <summary>
        /// Random numbers provider.
        /// </summary>
        protected IBattleRandomNumbersProvider RandomProvider => BattleManager.RandomProvider;
    }
}