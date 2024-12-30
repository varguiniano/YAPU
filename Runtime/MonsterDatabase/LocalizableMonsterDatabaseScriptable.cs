using Sirenix.OdinInspector;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase
{
    /// <summary>
    /// Base class for database scriptables that need localization.
    /// </summary>
    /// <typeparam name="T">Implementing class.</typeparam>
    public abstract class LocalizableMonsterDatabaseScriptable<T> : MonsterDatabaseScriptable<T>
        where T : LocalizableMonsterDatabaseScriptable<T>
    {
        /// <summary>
        /// Base localization root for the asset.
        /// </summary>
        protected abstract string BaseLocalizationRoot { get; }

        /// <summary>
        /// Auto fill the localization values.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        protected virtual void RefreshLocalizableNames()
        {
            LocalizableName = BaseLocalizationRoot + name;
            LocalizableDescription = LocalizableName + "/Description";
        }

        /// <summary>
        /// Localizable name for this asset.
        /// </summary>
        [FoldoutGroup("Localization")]
        public string LocalizableName;

        /// <summary>
        /// Localizable description of this asset.
        /// </summary>
        [FoldoutGroup("Localization")]
        [ShowIf(nameof(HasLocalizableDescription))]
        public string LocalizableDescription;

        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected abstract bool HasLocalizableDescription { get; }
    }
}