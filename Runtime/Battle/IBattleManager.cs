using System;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Interface that defines a manager that handles a battle.
    /// </summary>
    public interface IBattleManager
    {
        /// <summary>
        /// Subscribe to the event of the battle has initialized.
        /// </summary>
        /// <param name="callback">Callback when initialized.</param>
        void SubscribeToBattleInitialized(Action callback);
        
        /// <summary>
        /// Subscribe to the event of the battle has finished.
        /// </summary>
        /// <param name="callback">Callback when finished.</param>
        void SubscribeToBattleFinished(Action<BattleResultParameters> callback);
    }
}