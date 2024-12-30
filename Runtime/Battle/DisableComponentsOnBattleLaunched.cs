using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Disables a series of components when a battle is launched, reenables them on battle to unload.
    /// </summary>
    public class DisableComponentsOnBattleLaunched : WhateverBehaviour<DisableComponentsOnBattleLaunched>
    {
        /// <summary>
        /// Component to disable.
        /// </summary>
        [SerializeField]
        private List<Behaviour> Components;
        
        /// <summary>
        /// Reference to the battle launcher.
        /// </summary>
        [Inject]
        private IBattleLauncher battleLauncher;

        /// <summary>
        /// Subscribe to the launcher.
        /// </summary>
        private void OnEnable()
        {
            battleLauncher.SubscribeToBattleLaunched(DisableComponent);
            battleLauncher.SubscribeToBattleToUnload(EnableComponent);
        }

        /// <summary>
        /// Unsubscribe from the launcher.
        /// </summary>
        private void OnDisable()
        {
            battleLauncher.UnsubscribeFromBattleLaunched(DisableComponent);
            battleLauncher.UnsubscribeFromBattleToUnload(EnableComponent);
        }

        /// <summary>
        /// Enable the component.
        /// </summary>
        private void EnableComponent()
        {
            foreach (Behaviour component in Components) component.enabled = true;
        }

        /// <summary>
        /// Disable the component.
        /// </summary>
        private void DisableComponent()
        {
            foreach (Behaviour component in Components) component.enabled = false;
        }
    }
}