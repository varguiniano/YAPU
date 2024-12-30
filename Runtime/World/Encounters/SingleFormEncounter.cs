using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;

namespace Varguiniano.YAPU.Runtime.World.Encounters
{
    /// <summary>
    /// Most simple example of a form calculator, always returns the same form.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/Encounters/Calculators/SingleFormEncounter",
                     fileName = "SingleFormEncounter")]
    public class SingleFormEncounter : EncounterFormCalculator
    {
        /// <summary>
        /// Form to return.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        [SerializeField]
        [InfoBox("Form shouldn't be shiny! Shinies can appear by shiny chances.",
                 InfoMessageType.Error,
                 "@Form != null && Form.IsShiny")]
        private Form Form;

        /// <summary>
        /// Always return the same form.
        /// </summary>
        /// <param name="sceneInfo">Information on the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        /// <returns>The form to use.</returns>
        public override Form GetEncounterForm(SceneInfo sceneInfo, EncounterType encounterType) => Form;

        /// <summary>
        /// Get all the possible forms this encounter calculator can generate.
        /// Used for dex displaying.
        /// </summary>
        /// <returns>A list of all possible forms.</returns>
        public override List<Form> GetAllPossibleForms() => new() { Form };
    }
}