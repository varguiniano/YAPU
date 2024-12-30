using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Controller for the evolution animation.
    /// </summary>
    public class EvolutionAnimation : WhateverBehaviour<EvolutionAnimation>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the evolution music.
        /// </summary>
        [SerializeField]
        private AudioReference EvolutionMusic;

        /// <summary>
        /// Reference to the evolution music.
        /// </summary>
        [SerializeField]
        private AudioReference EvolutionSuccess;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [SerializeField]
        private MonsterSprite MonsterSprite;

        /// <summary>
        /// Reference to the shinny circle.
        /// </summary>
        [SerializeField]
        private Transform Circle;

        /// <summary>
        /// Reference to the particles effect.
        /// </summary>
        [SerializeField]
        private VisualEffect Particles;

        /// <summary>
        /// Reference to the evolution manager.
        /// </summary>
        private EvolutionManager evolutionManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the dex.
        /// </summary>
        private Dex dex;

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="evolutionManagerReference">Reference to the evolution manager.</param>
        /// <param name="localizerReference">Reference to the localizer.</param>
        /// <param name="audioManagerReference">Reference to the audio manager.</param>
        /// <param name="dexReference">Reference to the dex.</param>
        [Inject]
        public void Construct(EvolutionManager evolutionManagerReference,
                              ILocalizer localizerReference,
                              IAudioManager audioManagerReference,
                              Dex dexReference)
        {
            evolutionManager = evolutionManagerReference;
            localizer = localizerReference;
            audioManager = audioManagerReference;
            dex = dexReference;

            evolutionManager.RegisterAnimation(this);
        }

        /// <summary>
        /// Play the evolution animation.
        /// </summary>
        /// <param name="callback">Callback stating if the player cancelled the animation.</param>
        public IEnumerator PlayEvolutionAnimation(Action<bool> callback)
        {
            MonsterSprite.SetMonster(evolutionManager.MonsterToEvolve);

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            int choice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/GoAhead",
                                             "Common/Stop"
                                         },
                                         option => choice = option,
                                         onBackCallback: () => choice = 1,
                                         showDialog: true,
                                         localizationKey: "Dialogs/Monsters/WantsToEvolve",
                                         localizableModifiers: false,
                                         modifiers: evolutionManager.MonsterToEvolve.GetNameOrNickName(localizer));

            yield return new WaitUntil(() => choice != -1);

            if (choice == 1)
                yield return DialogManager.ShowDialogAndWait("Dialogs/Monsters/WontEvolve",
                                                             localizableModifiers: false,
                                                             modifiers: evolutionManager.MonsterToEvolve
                                                                .GetNameOrNickName(localizer));
            else
            {
                audioManager.StopAllAudios();

                DialogManager.ShowDialog("Dialogs/Monsters/IsEvolving",
                                         acceptInput: false,
                                         localizableModifiers: false,
                                         modifiers: evolutionManager.MonsterToEvolve.GetNameOrNickName(localizer));

                audioManager.PlayAudio(EvolutionMusic);
                Particles.enabled = true;

                Circle.DOScale(Vector3.one, .5f)
                      .OnComplete(() => Circle.DOShakeScale(1, new Vector3(.1f, .1f), randomness: 0).SetLoops(-1));

                yield return MonsterSprite.GetCachedComponent<Transform>()
                                          .DOScale(Vector3.zero, 5f)
                                          .WaitForCompletion();

                MonsterSprite.SetMonster(evolutionManager.NewSpecies,
                                         evolutionManager.NewForm,
                                         evolutionManager.MonsterToEvolve.PhysicalData.Gender,
                                         false,
                                         true,
                                         evolutionManager.MonsterToEvolve.ExtraData.PersonalityValue);

                yield return MonsterSprite.GetCachedComponent<Transform>().DOScale(Vector3.one, 5f).WaitForCompletion();

                Particles.Stop();

                Circle.DOKill();
                Circle.DOScale(Vector3.zero, .25f).SetEase(Ease.InBack);

                audioManager.StopAudio(EvolutionMusic);
                audioManager.PlayAudio(evolutionManager.NewSpecies[evolutionManager.NewForm].Cry);

                audioManager.PlayAudio(EvolutionSuccess);

                yield return DialogManager.WaitForTypewriter;
                DialogManager.NextDialog();

                string oldName = evolutionManager.MonsterToEvolve.GetNameOrNickName(localizer);

                evolutionManager.MonsterToEvolve.EvolveToSpeciesAndForm(evolutionManager.NewSpecies,
                                                                        evolutionManager.NewForm);

                dex.RegisterAsCaught(evolutionManager.MonsterToEvolve, true, true, false);

                yield return DialogManager.ShowDialogAndWait("Dialogs/Monsters/HasEvolved",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            oldName,
                                                                            localizer[evolutionManager.NewSpecies
                                                                               .LocalizableName]
                                                                        });

                foreach (Move move in evolutionManager.MonsterToEvolve.FormData.OnEvolutionMoves)
                    yield return DialogManager.ShowMoveLearnPanel(evolutionManager.MonsterToEvolve,
                                                                  move,
                                                                  localizer,
                                                                  _ =>
                                                                  {
                                                                  });

                audioManager.StopAudio(EvolutionSuccess, 1f);
            }

            callback.Invoke(choice == 1);

            yield return TransitionManager.BlackScreenFadeInRoutine();
        }
    }
}