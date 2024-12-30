using System.Collections.Generic;
using System.Linq;
using AssetIcons;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters
{
    /// <summary>
    /// Object representing a monster entry in the monster database.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Monster", fileName = "Monster")]
    public class MonsterEntry : DocumentableMonsterDatabaseScriptable<MonsterEntry>
    {
        /// <summary>
        /// All monster localizations should start with Monsters/.
        /// </summary>
        protected override string BaseLocalizationRoot => "Monsters/";

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;

        /// <summary>
        /// Auto fill the localization values.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        protected override void RefreshLocalizableNames()
        {
            string[] split = name.Split('-');

            if (split.Length < 2)
                LocalizableName = BaseLocalizationRoot + name;
            else
                LocalizableName = BaseLocalizationRoot + split[1];

            LocalizableDescription = LocalizableName + "/Description";
        }

        /// <summary>
        /// Number of this monster on the dex.
        /// </summary>
        public uint DexNumber;

        /// <summary>
        /// Available forms for this monster.
        /// </summary>
        [ValueDropdown("GetAllForms")]
        [OnCollectionChanged(nameof(OnAvailableFormsChanged))]
        [InfoBox("There are shiny forms in the available forms. You shouldn't add them; they are retrieved automatically from the normal ones.",
                 InfoMessageType.Error,
                 nameof(shinyFormsInAvailable))]
        [InfoBox("The entry needs at least one form.",
                 InfoMessageType.Error,
                 nameof(HasNoAvailableForms))]
        public List<Form> AvailableForms;

        /// <summary>
        /// Available forms for this monster, including shinny forms.
        /// </summary>
        public List<Form> AvailableFormsWithShinnies
        {
            get
            {
                List<Form> available = new(AvailableForms);
                available.AddRange(from form in AvailableForms where form.HasShinyVersion select form.ShinyVersion);
                return available;
            }
        }

        /// <summary>
        /// List of all the types by form.
        /// </summary>
        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField]
        private List<DataByFormEntry> DataByForm;

        /// <summary>
        /// Get the data for the given form.
        /// </summary>
        /// <param name="form">Form to check.</param>
        public DataByFormEntry this[Form form]
        {
            get
            {
                foreach (DataByFormEntry formEntry in DataByForm.Where(formEntry =>
                                                                           formEntry.Form == form
                                                                        || (formEntry.Form.HasShinyVersion
                                                                         && form == formEntry.Form.ShinyVersion)))
                    return formEntry;

                Logger.Error(name + " doesn't have the " + form.name + " form!");

                return null;
            }
        }

        /// <summary>
        /// Check if a form is available.
        /// </summary>
        /// <param name="form">Form to check.</param>
        /// <returns>True if available.</returns>
        public bool IsFormAvailable(Form form)
        {
            foreach (Form availableForm in AvailableForms)
            {
                if (availableForm == form) return true;
                if (availableForm.HasShinyVersion && availableForm.ShinyVersion == form) return true;
            }

            return false;
        }

        /// <summary>
        /// Get the first available form.
        /// </summary>
        /// <returns></returns>
        public Form GetFirstAvailableForm() => AvailableForms[0];

        /// <summary>
        /// Copy the form data from the source form to the target form.
        /// </summary>
        /// <param name="source">Source form.</param>
        /// <param name="target">Target form.</param>
        [Button]
        [FoldoutGroup("Utils")]
        [ShowIf("@" + nameof(AvailableForms) + ".Count > 1")]
        private void CopyDataFromFormToForm(Form source, Form target) =>
            CopyDataFromFormToForm(AvailableForms.IndexOf(source), AvailableForms.IndexOf(target));

        /// <summary>
        /// Copy the form data from the source form to the target form.
        /// </summary>
        /// <param name="source">Source form.</param>
        /// <param name="target">Target form.</param>
        [Button]
        [FoldoutGroup("Utils")]
        [ShowIf("@" + nameof(AvailableForms) + ".Count > 1")]
        private void CopyDataFromFormToForm(int source, int target = 1)
        {
            Form targetForm = DataByForm[target].Form;
            DataByForm[target] = DataByForm[source].Clone();
            DataByForm[target].Form = targetForm;
        }

        #region EditorChecks

        /// <summary>
        /// Does this entry have forms?
        /// </summary>
        private bool HasAvailableForms => AvailableForms is {Count: > 0};

        /// <summary>
        /// Does this entry have forms?
        /// </summary>
        private bool HasNoAvailableForms => !HasAvailableForms;

        /// <summary>
        /// Flag to note if there are shiny forms in the available forms.
        /// </summary>
        #pragma warning disable CS0414
        private bool shinyFormsInAvailable;
        #pragma warning restore CS0414

        #if UNITY_EDITOR

        /// <summary>
        /// Init the inspector.
        /// </summary>
        [OnInspectorInit]
        protected override void InspectorInit()
        {
            base.InspectorInit();

            if (DataByForm == null) return;

            foreach (DataByFormEntry entry in DataByForm.Where(entry => entry != null)) entry.InspectorInit();
        }

        #endif

        /// <summary>
        /// Called when the available forms have changed.
        /// </summary>
        private void OnAvailableFormsChanged()
        {
            shinyFormsInAvailable = false;

            foreach (Form _ in AvailableForms.Where(form => form.IsShiny)) shinyFormsInAvailable = true;

            RefreshTypes();
        }

        /// <summary>
        /// Refresh the types and sync them with the forms.
        /// </summary>
        private void RefreshTypes()
        {
            List<DataByFormEntry> toRemove = DataByForm.Where(entry => !AvailableForms.Contains(entry.Form)).ToList();

            foreach (DataByFormEntry entry in toRemove) DataByForm.Remove(entry);

            foreach (Form form in AvailableForms)
            {
                bool alreadySet = false;

                foreach (DataByFormEntry _ in DataByForm.Where(entry => entry.Form == form)) alreadySet = true;

                if (alreadySet) continue;

                DataByForm.Add(new DataByFormEntry {Form = form});
            }
        }

        #endregion

        /// <summary>
        /// Return the icon to use in the editor.
        /// </summary>
        [AssetIcon] // TODO: Optional.
        public Sprite EditorIcon
        {
            get
            {
                if (AvailableForms == null) return null;

                if (AvailableForms.Count == 0) return null;

                Form form = AvailableForms[0];

                DataByFormEntry dataByForm;

                try
                {
                    dataByForm = this[form];
                }
                catch
                {
                    return null;
                }

                return dataByForm?.Icon;
            }
        }
    }
}