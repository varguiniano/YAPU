using System;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;

namespace Varguiniano.YAPU.Runtime.Monster.Breeding
{
    /// <summary>
    /// Base class for a data structure that defines the offspring of a monster.
    /// </summary>
    [Serializable]
    public abstract class BreedingData : MonsterDatabaseData<BreedingData>
    {
        /// <summary>
        /// Get the species and form of the offspring.
        /// This method doesn't check egg groups.
        /// </summary>
        /// <param name="monster">Current monster.</param>
        /// <param name="otherParent">The other parent.</param>
        /// <returns>A list of tuples with the possible species and forms of the offspring.</returns>
        public abstract List<(MonsterEntry, Form)> GetPossibleOffspring(
            MonsterInstance monster,
            MonsterInstance otherParent);

        /// <summary>
        /// Get the relationships this breeding data can have to be displayed on the dex.
        /// </summary>
        /// <param name="entry">Monster entry being displayed.</param>
        /// <param name="formEntry">Form entry being displayed.</param>
        /// <returns>A list of the relationships generated.</returns>
        public abstract List<DexMonsterRelationshipData> GetDexRelationships(
            MonsterDexEntry entry,
            FormDexEntry formEntry);

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public abstract BreedingData Clone();
    }
}