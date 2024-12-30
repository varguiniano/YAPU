using System;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Data recording the last damage move that hit a battler.
    /// </summary>
    [Serializable]
    public class LastDamageMoveReceivedData
    {
        /// <summary>
        /// Move that hit the battler.
        /// </summary>
        public Move Move;

        /// <summary>
        /// Damage the move dealt.
        /// </summary>
        public int Damage;

        /// <summary>
        /// User of that move.
        /// </summary>
        public Battler User;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="move">Move that hit the battler.</param>
        /// <param name="damage">Damage the move dealt.</param>
        /// <param name="user">User of that move.</param>
        public LastDamageMoveReceivedData(Move move, int damage, Battler user)
        {
            Move = move;
            Damage = damage;
            User = user;
        }
    }
}