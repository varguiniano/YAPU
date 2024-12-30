using System;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Data class to store whether a monster has the virus.
    /// </summary>
    [Serializable]
    public struct VirusData
    {
        /// <summary>
        /// Does the monster has the virus.
        /// </summary>
        public bool HasVirus;

        /// <summary>
        /// Did this monster pass the virus and is now immune?
        /// </summary>
        public bool IsImmune;

        /// <summary>
        /// Virus effects affect the monster even after cured
        /// but it will no longer infect others.
        /// </summary>
        public bool GetsVirusEffect => HasVirus || IsImmune;

        /// <summary>
        /// Can this monster get infected?
        /// </summary>
        public bool CanGetInfected => !HasVirus && !IsImmune;

        /// <summary>
        /// Strain of the infected virus.
        /// </summary>
        public byte Strain;

        /// <summary>
        /// If this timer is over 0, the virus can be infected to adjacent monsters.
        /// </summary>
        public byte InfectionTimer;

        /// <summary>
        /// Constructor for a monster's virus data.
        /// </summary>
        /// <param name="hasVirus">Does the monster have the virus?</param>
        /// <param name="isImmune">Is it immune to the virus?</param>
        /// <param name="strain">What strain of the virus does it have?</param>
        internal VirusData(bool hasVirus, bool isImmune = false, int strain = -1)
        {
            IsImmune = isImmune;

            if (strain is < 0 or > 3) strain = Random.Range(0, 4);

            if (IsImmune || !hasVirus)
            {
                HasVirus = false;
                Strain = 0;
                InfectionTimer = 0;
            }
            else
            {
                HasVirus = true;
                Strain = (byte)strain;
                InfectionTimer = Strain;
            }
        }

        /// <summary>
        /// Get infected.
        /// </summary>
        public void Infect()
        {
            HasVirus = true;
            Strain = (byte)Random.Range(1, 5);
            InfectionTimer = Strain;
        }

        /// <summary>
        /// Tick the infection.
        /// </summary>
        public void InfectionTick()
        {
            InfectionTimer--;

            if (InfectionTimer != 0) return;
            HasVirus = false;
            IsImmune = true;
        }
    }
}