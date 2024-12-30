using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;

namespace Varguiniano.YAPU.Runtime.World.Encounters
{
    /// <summary>
    /// Abstract class for calculators that determine the form of a monster to be used in an encounter.
    /// </summary>
    public abstract class EncounterFormCalculator : MonsterDatabaseScriptable<EncounterFormCalculator>
    {
        /// <summary>
        /// Calculate the form to be used in an encounter.
        /// </summary>
        /// <param name="sceneInfo">Information on the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        /// <returns>The form to use.</returns>
        public abstract Form GetEncounterForm(SceneInfo sceneInfo, EncounterType encounterType);

        /// <summary>
        /// Get all the possible forms this encounter calculator can generate.
        /// Used for dex displaying.
        /// </summary>
        /// <returns>A list of all possible forms.</returns>
        public abstract List<Form> GetAllPossibleForms();
    }
}