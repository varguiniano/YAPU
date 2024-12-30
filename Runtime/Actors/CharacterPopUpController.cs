using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Controller for popups that show on top of characters.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterPopUpController : WhateverBehaviour<CharacterPopUpController>
    {
        /// <summary>
        /// Reference to the exclamation sprite.
        /// </summary>
        [SerializeField]
        private Sprite Exclamation;

        /// <summary>
        /// Reference to the exclamation audio.
        /// </summary>
        [SerializeField]
        private AudioReference ExclamationAudio;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Play the exclamation popup.
        /// </summary>
        [Button]
        [HideInEditorMode]
        private void TestPlayExclamation() => StartCoroutine(PlayExclamation());

        /// <summary>
        /// Play the exclamation popup.
        /// </summary>
        public IEnumerator PlayExclamation()
        {
            audioManager.PlayAudio(ExclamationAudio);
            yield return PlayPopup(Exclamation);
        }

        /// <summary>
        /// Play the popup with the given sprite.
        /// </summary>
        /// <param name="popup">Sprite to use as popup.</param>
        private IEnumerator PlayPopup(Sprite popup)
        {
            yield return GetCachedComponent<Transform>().DOScaleY(0, 0).WaitForCompletion();
            yield return GetCachedComponent<SpriteRenderer>().DOFade(0, 0).WaitForCompletion();
            GetCachedComponent<SpriteRenderer>().sprite = popup;

            yield return GetCachedComponent<SpriteRenderer>().DOFade(1, 0).WaitForCompletion();

            yield return GetCachedComponent<Transform>().DOScaleY(1, .15f).SetEase(Ease.OutBounce);

            yield return new WaitForSeconds(.5f);

            yield return GetCachedComponent<SpriteRenderer>().DOFade(0, .05f);
        }
    }
}