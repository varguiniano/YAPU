using System.Collections;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Monster.Trade
{
    /// <summary>
    /// Controller for the trade animation.
    /// </summary>
    public class TradeAnimation : WhateverBehaviour<TradeAnimation>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the trade music.
        /// </summary>
        [SerializeField]
        private AudioReference TradeMusic;

        /// <summary>
        /// Reference to the trade music.
        /// </summary>
        [SerializeField]
        private AudioReference TradeSuccess;

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
        /// Reference to the trade manager.
        /// </summary>
        private TradeManager tradeManager;

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
        /// <param name="tradeManagerReference">Reference to the trade manager.</param>
        /// <param name="localizerReference">Reference to the localizer.</param>
        /// <param name="audioManagerReference">Reference to the audio manager.</param>
        /// <param name="dexReference">Reference to the dex.</param>
        [Inject]
        public void Construct(TradeManager tradeManagerReference,
                              ILocalizer localizerReference,
                              IAudioManager audioManagerReference,
                              Dex dexReference)
        {
            tradeManager = tradeManagerReference;
            localizer = localizerReference;
            audioManager = audioManagerReference;
            dex = dexReference;

            tradeManager.RegisterAnimation(this);
        }

        /// <summary>
        /// Play the trade animation.
        /// </summary>
        public IEnumerator PlayTradeAnimation()
        {
            MonsterSprite.SetMonster(tradeManager.MonsterToTrade);

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            audioManager.StopAllAudios();

            audioManager.PlayAudio(tradeManager.MonsterToTrade.FormData.Cry);
            audioManager.PlayAudio(TradeMusic);

            yield return new WaitForSeconds(1f);

            yield return DialogManager.ShowDialogAndWait("Dialogs/Monsters/Trade/Goodbye",
                                                         switchToNextAfterSeconds: 1.5f,
                                                         localizableModifiers: false,
                                                         modifiers: tradeManager.MonsterToTrade
                                                            .GetNameOrNickName(localizer));

            Circle.DOScale(new Vector3(1.7f, 1.7f, 1f), .5f)
                  .OnComplete(() => Circle.DOShakeScale(1, new Vector3(.1f, .1f)).SetLoops(-1));

            DialogManager.ShowDialog("Dialogs/Monsters/Trade/Sent",
                                     switchToNextAfterSeconds: 1.5f,
                                     localizableModifiers: false,
                                     modifiers: new[]
                                                {
                                                    tradeManager.MonsterToTrade
                                                                .GetNameOrNickName(localizer),
                                                    tradeManager.Recipient.GetLocalizedFullName(localizer)
                                                });

            yield return MonsterSprite.GetCachedComponent<Transform>()
                                      .DOMoveY(2.5f, 5f)
                                      .WaitForCompletion();

            MonsterSprite.SetMonster(tradeManager.NewMonster);

            yield return MonsterSprite.GetCachedComponent<Transform>().DOMoveY(0, 5f).WaitForCompletion();

            Circle.DOKill();
            Circle.DOScale(Vector3.zero, .25f).SetEase(Ease.InBack);

            audioManager.StopAudio(TradeMusic);
            audioManager.PlayAudio(tradeManager.NewMonster.FormData.Cry);
            audioManager.PlayAudio(TradeSuccess);

            yield return DialogManager.ShowDialogAndWait("Dialogs/Monsters/Trade/Received",
                                                         switchToNextAfterSeconds: 5.5f,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        tradeManager.NewMonster
                                                                           .GetNameOrNickName(localizer),
                                                                        tradeManager.Recipient
                                                                           .GetLocalizedName(localizer)
                                                                    });

            dex.RegisterAsCaught(tradeManager.NewMonster, false, false, false);

            yield return TransitionManager.BlackScreenFadeInRoutine();
        }
    }
}