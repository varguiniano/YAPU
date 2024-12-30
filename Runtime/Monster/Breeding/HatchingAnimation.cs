using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Monster.Breeding
{
    /// <summary>
    /// Controller for the hatching animation.
    /// </summary>
    public class HatchingAnimation : WhateverBehaviour<HatchingAnimation>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the Hatching music.
        /// </summary>
        [SerializeField]
        private AudioReference HatchingMusic;

        /// <summary>
        /// Reference to the Hatching music.
        /// </summary>
        [SerializeField]
        private AudioReference HatchingSuccess;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [SerializeField]
        private MonsterSprite MonsterSprite;

        /// <summary>
        /// Reference to the particles effect.
        /// </summary>
        [SerializeField]
        private VisualEffect Particles;

        /// <summary>
        /// Reference to the hatching manager.
        /// </summary>
        private HatchingManager hatchingManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the dex.
        /// </summary>
        private Dex dex;

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="hatchingManagerReference">Reference to the hatching manager.</param>
        /// <param name="localizerReference">Reference to the localizer.</param>
        /// <param name="audioManagerReference">Reference to the audio manager.</param>
        /// <param name="configurationManagerReference">Reference to the configuration manager.</param>
        /// <param name="inputManagerReference">Reference to the input manager.</param>
        /// <param name="yapuSettings">Reference to the YAPU settings.</param>
        /// <param name="dexReference">Reference to the dex.</param>
        [Inject]
        public void Construct(HatchingManager hatchingManagerReference,
                              ILocalizer localizerReference,
                              IAudioManager audioManagerReference,
                              IConfigurationManager configurationManagerReference,
                              IInputManager inputManagerReference,
                              YAPUSettings yapuSettings,
                              Dex dexReference)
        {
            hatchingManager = hatchingManagerReference;
            localizer = localizerReference;
            audioManager = audioManagerReference;
            configurationManager = configurationManagerReference;
            inputManager = inputManagerReference;
            settings = yapuSettings;
            dex = dexReference;

            hatchingManager.RegisterAnimation(this);
        }

        /// <summary>
        /// Play the evolution animation.
        /// </summary>
        public IEnumerator PlayHatchingAnimation()
        {
            MonsterSprite.SetMonster(hatchingManager.MonsterToHatch);

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            audioManager.StopAllAudios();

            audioManager.PlayAudio(HatchingMusic);

            yield return new WaitForSeconds(.5f);

            Particles.EnableAndPlay();

            yield return MonsterSprite.GetCachedComponent<Transform>().DOScale(Vector3.zero, 5f).WaitForCompletion();

            hatchingManager.MonsterToHatch.EggData.IsEgg = false;

            MonsterSprite.SetMonster(hatchingManager.MonsterToHatch);

            yield return MonsterSprite.GetCachedComponent<Transform>().DOScale(Vector3.one, 5f).WaitForCompletion();

            Particles.Stop();

            audioManager.StopAudio(HatchingMusic);
            audioManager.PlayAudio(hatchingManager.MonsterToHatch.FormData.Cry);

            audioManager.PlayAudio(HatchingSuccess);

            dex.RegisterAsCaught(hatchingManager.MonsterToHatch, false, false, true);

            yield return DialogManager.WaitForTypewriter;
            DialogManager.NextDialog();

            yield return DialogManager.ShowDialogAndWait("Dialogs/Hatching/HasHatched",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        hatchingManager.MonsterToHatch
                                                                           .GetNameOrNickName(localizer)
                                                                    });

            if (!configurationManager.GetConfiguration(out GameplayConfiguration configuration))
            {
                Logger.Error("Couldn't retrieve gameplay configuration!");
                yield break;
            }

            if (configuration.ShowNicknameDialog)
            {
                int option = -1;

                DialogManager.ShowChoiceMenu(new List<string>
                                             {
                                                 "Common/True",
                                                 "Common/False"
                                             },
                                             choice => option = choice,
                                             onBackCallback: () => option = 1,
                                             showDialog: true,
                                             localizationKey: "Dialogs/StoringPrompt/WantToNickname",
                                             localizableModifiers: false,
                                             modifiers: hatchingManager.MonsterToHatch.GetNameOrNickName(localizer));

                yield return new WaitWhile(() => option == -1);

                if (option == 0)
                {
                    yield return DialogManager.RequestTextInput(settings.MaxNicknameSize,
                                                                "Dialogs/TextInput/Nickname",
                                                                new[]
                                                                {
                                                                    hatchingManager.MonsterToHatch
                                                                       .GetNameOrNickName(localizer)
                                                                },
                                                                hatchingManager.MonsterToHatch.GetIcon(),
                                                                (entered, text) =>
                                                                {
                                                                    if (entered)
                                                                        hatchingManager.MonsterToHatch.Nickname = text;

                                                                    hatchingManager.MonsterToHatch.HasNickname =
                                                                        entered;
                                                                });

                    yield return TransitionManager.BlackScreenFadeOutRoutine();

                    inputManager.BlockInput(false);
                }
            }

            audioManager.StopAudio(HatchingSuccess, 1f);

            yield return TransitionManager.BlackScreenFadeInRoutine();
        }
    }
}