using System;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.Monster.Saves
{
    /// <summary>
    /// Savable version of the origin data struct.
    /// </summary>
    [Serializable]
    public class SavableOriginData
    {
        /// <summary>
        /// Game in which it was obtained.
        /// </summary>
        public string Game;

        /// <summary>
        /// Region in which it was obtained.
        /// </summary>
        public string Region;

        /// <summary>
        /// Location in which it was obtained.
        /// </summary>
        public string Location;

        /// <summary>
        /// Original trainer.
        /// </summary>
        public string Trainer;

        /// <summary>
        /// Level when obtained.
        /// </summary>
        public byte OriginalLevel;

        /// <summary>
        /// Origin type of this monster.
        /// </summary>
        public OriginData.Type OriginType;

        /// <summary>
        /// Is this an alpha monster?
        /// </summary>
        public bool IsAlpha;

        /// <summary>
        /// Ball it was captured with.
        /// </summary>
        public int Ball;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="originData">Original origin data struct.</param>
        public SavableOriginData(OriginData originData)
        {
            Game = originData.Game;
            Region = originData.Region;
            Location = originData.Location;
            Trainer = originData.Trainer;
            OriginalLevel = originData.OriginalLevel;
            OriginType = originData.OriginType;
            IsAlpha = originData.IsAlpha;
            Ball = originData.Ball.name.GetHashCode();
        }
    }
}