using UnityEngine;
using UnityEngine.SceneManagement;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Initialization
{
    /// <summary>
    /// Load a scene after YAPU has been initialized.
    /// </summary>
    [RequireComponent(typeof(YAPUInitialization))]
    public class LoadMainMenuOrGameAfterYAPUInit : WhateverBehaviour<LoadMainMenuOrGameAfterYAPUInit>
    {
        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        [Inject]
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Subscribe to the init event.
        /// </summary>
        private void OnEnable()
        {
            if (!configurationManager.GetConfiguration(out GameplayConfiguration configuration))
            {
                Logger.Error("Error retrieving configuration.");
                return;
            }

            GetCachedComponent<YAPUInitialization>().Initialized += () =>
                                                                    {
                                                                        if (configuration
                                                                               .SkipMainMenuWhenOpeningGame
                                                                         && SavegameManager
                                                                               .AreThereSavegames())
                                                                            CoroutineRunner
                                                                               .RunRoutine(savegameManager
                                                                                   .LoadLastSavegame());
                                                                        else
                                                                            sceneManager
                                                                               .LoadScene(settings
                                                                                       .MainMenuScene,
                                                                                    _ =>
                                                                                    {
                                                                                    },
                                                                                    _ =>
                                                                                    {
                                                                                        DialogManager
                                                                                           .ShowLoadingIcon(false);

                                                                                        TransitionManager
                                                                                           .BlackScreenFadeOut();
                                                                                    },
                                                                                    LoadSceneMode.Single);
                                                                    };
        }
    }
}