using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Player
{
    /// <summary>
    /// Data class that stores general gameplay settings the player can set per game.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Player/Settings", fileName = "PlayerSettings")]
    public class PlayerSettings : WhateverScriptable<YAPUSettings>
    {
        /// <summary>
        /// Does all the team gain XP when an enemy faints?
        /// </summary>
        [FoldoutGroup("Experience")]
        public bool AllTeamGainsXPOnFaint;

        /// <summary>
        /// Does catching yield XP?
        /// </summary>
        [FoldoutGroup("Experience")]
        public bool CatchingYieldsXP;
    }
}