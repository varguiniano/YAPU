using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs
{
    /// <summary>
    /// Installer for the dialog manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Dialog Manager Installer",
                     fileName = "DialogManagerInstaller")]
    public class DialogManagerInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the dialog manager prefab.
        /// </summary>
        public DialogManager Prefab;

        /// <summary>
        /// Instantiate the singleton and install the reference.
        /// </summary>
        public override void InstallBindings() =>
            Container.BindFactory<DialogManager, DialogManager.Factory>().FromComponentInNewPrefab(Prefab);
    }
}