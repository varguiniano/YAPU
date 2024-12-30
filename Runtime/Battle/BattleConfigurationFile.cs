using System;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Class representing the configuration file for battles.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/Configuration", fileName = "BattleConfiguration")]
    public class BattleConfigurationFile : ConfigurationScriptableHolderUsingFirstValidPersister<BattleConfiguration>
    {
    }

    /// <summary>
    /// Configuration data for battles.
    /// </summary>
    [Serializable]
    public class BattleConfiguration : ConfigurationData
    {
        /// <summary>
        /// Battle style to use.
        /// </summary>
        public BattleStyle BattleStyle;

        /// <summary>
        /// Multiplier for the animation speed.
        /// This should be between 1 and not an insane amount.
        /// </summary>
        [Range(1, 8)]
        public byte BattleAnimationSpeedUp = 1;

        /// <summary>
        /// Clone this data into a new instance of the same type.
        /// </summary>
        /// <typeparam name="TConfigurationData">Type of the configuration.</typeparam>
        /// <returns>The cloned config.</returns>
        protected override TConfigurationData Clone<TConfigurationData>() =>
            new BattleConfiguration
            {
                BattleStyle = BattleStyle,
                BattleAnimationSpeedUp = BattleAnimationSpeedUp
            } as TConfigurationData;
    }
}