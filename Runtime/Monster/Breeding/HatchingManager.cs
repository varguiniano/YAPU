using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Monster.Breeding
{
    /// <summary>
    /// Controller for hatching eggs.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Breeding/Manager", fileName = "HatchingManager")]
    public class HatchingManager : WhateverScriptable<HatchingManager>
    {
        /// <summary>
        /// Monster that it's about to hatch.
        /// </summary>
        [ReadOnly]
        public MonsterInstance MonsterToHatch;

        /// <summary>
        /// Reference to the Hatching scene.
        /// </summary>
        [SerializeField]
        private SceneReference HatchingScene;

        /// <summary>
        /// Reference to the select audio.
        /// </summary>
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        private GlobalGridManager globalGridManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the hatching animation in the scene.
        /// </summary>
        private HatchingAnimation hatchingAnimation;

        /// <summary>
        /// Allow the hatching animation to register itself to the manager.
        /// </summary>
        /// <param name="hatchingAnimationReference">Reference to the animation.</param>
        public void RegisterAnimation(HatchingAnimation hatchingAnimationReference) =>
            hatchingAnimation = hatchingAnimationReference;

        /// <summary>
        /// Trigger egg hatching after an egg cycle.
        /// </summary>
        /// <param name="monsters">Monster list.</param>
        public IEnumerator TriggerHatchingAfterCycle(IEnumerable<MonsterInstance> monsters)
        {
            // ReSharper disable once WrongIndentSize
            List<MonsterInstance> monstersToHatch = monsters.Where(monster => monster is
                                                             {
                                                                 IsNullEntry: false,
                                                                 EggData:
                                                                 {
                                                                     IsEgg: true,
                                                                     EggCyclesLeft: 0
                                                                 }
                                                             })
                                                            .ToList();
            
            foreach (MonsterInstance monster in monstersToHatch)
            {
                Logger.Info("Triggering " + monster.GetNameOrNickName(localizer) + " hatching.");

                audioManager.PlayAudio(SelectAudio);

                yield return DialogManager.ShowDialogAndWait("Dialogs/Hatching/Intro");

                inputManager.BlockInput();

                hatchingAnimation = null;

                MonsterToHatch = monster;

                yield return TransitionManager.BlackScreenFadeInRoutine();

                bool sceneLoaded = false;

                sceneManager.LoadScene(HatchingScene, null, _ => sceneLoaded = true);

                yield return new WaitUntil(() => sceneLoaded);

                // By this point the animation reference has been assigned.
                // ReSharper disable once PossibleNullReferenceException
                yield return hatchingAnimation.PlayHatchingAnimation();

                bool sceneUnloaded = false;

                sceneManager.UnloadScene(HatchingScene, null, _ => sceneUnloaded = true);

                yield return new WaitUntil(() => sceneUnloaded);

                MonsterToHatch = null;
                
                TransitionManager.BlackScreenFadeOut();
                globalGridManager.PlayCurrentSceneMusic();
                
                inputManager.BlockInput(false);
            }
        }
    }
}