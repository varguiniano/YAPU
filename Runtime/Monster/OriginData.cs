using System;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Class to store the origin data of a monster.
    /// </summary>
    [Serializable]
    public struct OriginData
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
        public Type OriginType;

        /// <summary>
        /// Is this an alpha monster?
        /// </summary>
        public bool IsAlpha;

        /// <summary>
        /// Localization key for the origin type.
        /// </summary>
        public string OriginTypeLocalizationKey => "Monsters/Origin/" + OriginType;

        /// <summary>
        /// Ball it was captured with.
        /// </summary>
        public Ball Ball;

        /// <summary>
        /// Origin types a monster can have.
        /// </summary>
        public enum Type
        {
            Caught,
            Hatched,
            Resurrected,
            Unknown
        }
    }
}