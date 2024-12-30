using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Forms
{
    /// <summary>
    /// Object representing a monster form.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Form", fileName = "Form")]
    public class Form : LocalizableMonsterDatabaseScriptable<Form>
    {
        /// <summary>
        /// All form localization should start with Forms/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Forms/";
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;

        /// <summary>
        /// Is this form shiny?
        /// </summary>
        [HideIf("@ShinyVersion != null")]
        public bool IsShiny;

        /// <summary>
        /// Shiny version of this form is it should have.
        /// </summary>
        [HideIf(nameof(IsShiny))]
        public Form ShinyVersion;

        /// <summary>
        /// Does this form have a shiny version?
        /// </summary>
        public bool HasShinyVersion => !IsShiny && ShinyVersion != null;

        /// <summary>
        /// Is this a combat form?
        /// </summary>
        public bool IsCombatForm;

        /// <summary>
        /// Type of combat form.
        /// </summary>
        [ShowIf("IsCombatForm")]
        public CombatForm CombatFormType;

        /// <summary>
        /// Is this form a mega form?
        /// </summary>
        public bool IsMegaForm => IsCombatForm && CombatFormType == CombatForm.Mega;

        /// <summary>
        /// Combat form types.
        /// </summary>
        public enum CombatForm
        {
            Mega = 0,
            Max = 1,
            Other = 255
        }
    }
}