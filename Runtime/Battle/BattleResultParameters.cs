using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Parameters sent back by the battle launcher when the battle is over. 
    /// </summary>
    [Serializable]
    public class BattleResultParameters
    {
        /// <summary>
        /// The type of battle that was fought.
        /// </summary>
        public BattleType BattleType;

        /// <summary>
        /// Whether the player won.
        /// </summary>
        public bool PlayerWon;

        /// <summary>
        /// The player roster, which has been modified during the battle.
        /// Ex. Level ups, HP loss.
        /// </summary>
        public List<Battler> PlayerRoster;

        /// <summary>
        /// Captured monster.
        /// </summary>
        public MonsterInstance CapturedMonster;

        /// <summary>
        /// Respawn the player if they lose?
        /// </summary>
        public bool RespawnIfLose;
    }
}