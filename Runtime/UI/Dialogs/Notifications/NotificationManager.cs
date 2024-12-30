using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.Notifications
{
    /// <summary>
    /// Behaviour in charge of showing notifications to the player.
    /// </summary>
    public class NotificationManager : WhateverBehaviour<NotificationManager>
    {
        /// <summary>
        /// Reference to the saving notification.
        /// </summary>
        [FoldoutGroup(nameof(SavingNotification))]
        [SerializeField]
        private HidableUiElement SavingNotification;

        /// <summary>
        /// Position of the saving notification when open.
        /// </summary>
        [FoldoutGroup(nameof(SavingNotification))]
        [SerializeField]
        private Transform SavingOpen;

        /// <summary>
        /// Position of the saving notification when closed.
        /// </summary>
        [FoldoutGroup(nameof(SavingNotification))]
        [SerializeField]
        private Transform SavingClosed;

        /// <summary>
        /// Icon for when a scene is entered.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification) + "/Icons")]
        [SerializeField]
        private Sprite SceneEnteredIcon;

        /// <summary>
        /// Reference to the icon text notification.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification))]
        [SerializeField]
        private HidableUiElement IconTextNotification;

        /// <summary>
        /// Reference to the icon text notification icon.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification))]
        [SerializeField]
        private Image Icon;

        /// <summary>
        /// Reference to the icon text notification text.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification))]
        [SerializeField]
        private LocalizedTextMeshPro Text;

        /// <summary>
        /// Reference to the icon text notification open position.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification))]
        [SerializeField]
        private Transform IconTextOpen;

        /// <summary>
        /// Reference to the icon text notification closed position.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification))]
        [SerializeField]
        private Transform IconTextClosed;

        /// <summary>
        /// Time notifications are shown.
        /// </summary>
        [FoldoutGroup(nameof(IconTextNotification))]
        [SerializeField]
        private float ShowDuration;

        /// <summary>
        /// Reference to the transform of the saving notification.
        /// </summary>
        private Transform SavingTransform
        {
            get
            {
                if (savingTransform == null) savingTransform = SavingNotification.transform;
                return savingTransform;
            }
        }

        /// <summary>
        /// Backfield for SavingTransform.
        /// </summary>
        private Transform savingTransform;

        /// <summary>
        /// Reference to the transform of the icon text notification.
        /// </summary>
        private Transform IconTextTransform
        {
            get
            {
                if (iconTextTransform == null) iconTextTransform = IconTextNotification.transform;
                return iconTextTransform;
            }
        }

        /// <summary>
        /// Backfield for IconTextTransform.
        /// </summary>
        private Transform iconTextTransform;

        /// <summary>
        /// Queue of notifications to display.
        /// </summary>
        private readonly Queue<IconTextNotificationData> notificationQueue = new();

        /// <summary>
        /// Multiplier to use on the duration of the notification depending on the queue size.
        /// Only applies when there are more than 2 notifications.
        /// </summary>
        private float NotificationDurationMultiplier =>
            notificationQueue.Count > 2 ? 1f / (notificationQueue.Count + 1) : 1;

        /// <summary>
        /// Start the queue routine.
        /// </summary>
        private void OnEnable() => StartCoroutine(ProcessIconTextNotificationQueue());

        /// <summary>
        /// Show or hide the saving notification.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        public IEnumerator ShowSavingNotification(bool show = true)
        {
            if (SavingNotification.Shown == show) yield break;

            if (show)
            {
                SavingNotification.Show(false);

                SetPosition(SavingTransform, SavingClosed);

                SavingNotification.Show();

                yield return SlideNotificationIn(SavingTransform, SavingOpen);
            }
            else
                yield return SlideNotificationOut(SavingTransform,
                                                  SavingClosed,
                                                  .1f,
                                                  () => SavingNotification.Show(false));
        }

        /// <summary>
        /// Queue a notification for entering a scene.
        /// </summary>
        /// <param name="sceneNameLocalizationKey">Localization key of the entered scene.</param>
        public void QueueSceneEnteredNotification(string sceneNameLocalizationKey) =>
            notificationQueue.Enqueue(new IconTextNotificationData
                                      {
                                          Icon = SceneEnteredIcon,
                                          Text = sceneNameLocalizationKey
                                      });

        /// <summary>
        /// Queue a notification with an icon and text.
        /// </summary>
        /// <param name="icon">Icon to show.</param>
        /// <param name="text">Text.</param>
        /// <param name="localizableText">Is the text localizable?</param>
        /// <param name="localizableModifiers">Are the text modifiers localizable keys? Not used if the text is not localizable.</param>
        /// <param name="dontDuplicate">If there is already a notification with the same text and icon on the queue or displaying, don't add another.</param>
        /// <param name="modifiers">Modifiers to apply to the text. Not used if the text is not localizable.</param>
        public void QueueIconTextNotification(Sprite icon,
                                              string text,
                                              bool localizableText = true,
                                              bool localizableModifiers = true,
                                              bool dontDuplicate = false,
                                              params string[] modifiers)
        {
            IconTextNotificationData notificationData = new()
                                                        {
                                                            Icon = icon,
                                                            Text = text,
                                                            LocalizableText = localizableText,
                                                            LocalizableModifiers = localizableModifiers,
                                                            Modifiers = modifiers
                                                        };

            if (dontDuplicate)
            {
                if (IconTextNotification.Shown && Icon.sprite == icon && Text.Text.text == text) return;

                if (notificationQueue.Any(data => data.Equals(notificationData))) return;
            }

            notificationQueue.Enqueue(notificationData);
        }

        /// <summary>
        /// Stop all notifications in the queue and clear it.
        /// </summary>
        public void StopAllNotifications() => notificationQueue.Clear();

        /// <summary>
        /// Routine in charge of processing the icon text notification queue.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ProcessIconTextNotificationQueue()
        {
            while (true)
            {
                while (notificationQueue.TryDequeue(out IconTextNotificationData notification))
                {
                    yield return ShowIconTextNotification(true,
                                                          notification.Icon,
                                                          notification.Text,
                                                          notification.LocalizableText,
                                                          notification.LocalizableModifiers,
                                                          notification.Modifiers);

                    yield return new WaitForSeconds(ShowDuration * NotificationDurationMultiplier);

                    yield return ShowIconTextNotification(false);
                }

                yield return WaitAFrame;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Show or hide a notification with an icon and text.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        /// <param name="icon">Icon to show.</param>
        /// <param name="text">Text to show, can be a localization key or an already localized text.</param>
        /// <param name="localizableText">Is the text localizable?</param>
        /// <param name="localizableModifiers">Are the modifiers localizable keys?</param>
        /// <param name="modifiers">Modifiers to apply to the text.</param>
        private IEnumerator ShowIconTextNotification(bool show,
                                                     Sprite icon = null,
                                                     string text = "",
                                                     bool localizableText = true,
                                                     bool localizableModifiers = true,
                                                     params string[] modifiers)
        {
            if (IconTextNotification.Shown == show) yield break;

            if (show)
            {
                IconTextNotification.Show(false);

                SetPosition(IconTextTransform, IconTextClosed);

                Icon.sprite = icon;

                if (localizableText)
                    Text.SetValue(text, localizableModifiers, modifiers);
                else
                    Text.Text.SetText(text);

                IconTextNotification.Show();

                yield return SlideNotificationIn(IconTextTransform, IconTextOpen);
            }
            else
                yield return SlideNotificationOut(IconTextTransform,
                                                  IconTextClosed,
                                                  onFinished: () => IconTextNotification.Show(false));
        }

        /// <summary>
        /// Slide a notification in.
        /// </summary>
        /// <param name="notification">Notification to slide.</param>
        /// <param name="openPosition">Target position.</param>
        /// <param name="duration">Duration of the slide.</param>
        /// <param name="onFinished">Called when finished.</param>
        private IEnumerator SlideNotificationIn(Transform notification,
                                                Transform openPosition,
                                                float duration = .25f,
                                                Action onFinished = null) =>
            SlideNotification(notification, openPosition, duration, Ease.OutBack, onFinished);

        /// <summary>
        /// Slide a notification out.
        /// </summary>
        /// <param name="notification">Notification to slide.</param>
        /// <param name="closedPosition">Target position.</param>
        /// <param name="duration">Duration of the slide.</param>
        /// <param name="onFinished">Called when finished.</param>
        private IEnumerator SlideNotificationOut(Transform notification,
                                                 Transform closedPosition,
                                                 float duration = .25f,
                                                 Action onFinished = null) =>
            SlideNotification(notification, closedPosition, duration, Ease.InBack, onFinished);

        /// <summary>
        /// Slide a notification in or out.
        /// </summary>
        /// <param name="notification">Notification to slide.</param>
        /// <param name="newPosition">Target position.</param>
        /// <param name="duration">Duration of the slide.</param>
        /// <param name="easing">Easing to use.</param>
        /// <param name="onFinished">Called when finished.</param>
        private IEnumerator SlideNotification(Transform notification,
                                              Transform newPosition,
                                              float duration,
                                              Ease easing,
                                              Action onFinished)
        {
            yield return notification.DOLocalMove(newPosition.localPosition, duration * NotificationDurationMultiplier)
                                     .SetEase(easing)
                                     .OnComplete(() => onFinished?.Invoke())
                                     .WaitForCompletion();
        }

        /// <summary>
        /// Set the position of a notification.
        /// </summary>
        /// <param name="notification">Notification to affect.</param>
        /// <param name="newPosition">Its new position.</param>
        private static void SetPosition(Transform notification, Transform newPosition) =>
            notification.localPosition = newPosition.localPosition;
    }
}