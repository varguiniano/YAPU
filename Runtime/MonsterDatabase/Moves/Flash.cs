using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Flash.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Flash", fileName = "Flash")]
    public class Flash : StageChangeMove
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject AnimationPrefab;

        /// <summary>
        /// Normal value for midtones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float NormalTone;

        /// <summary>
        /// Highest value for the midtones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float HightestTone;

        /// <summary>
        /// Audio for the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            Volume volume = Instantiate(AnimationPrefab).GetComponent<Volume>();

            volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

            effect.shadows.overrideState = true;
            effect.midtones.overrideState = true;
            effect.highlights.overrideState = true;

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            float tone = NormalTone;

            bool finished = false;

            DOTween.To(() => tone,
                       x => tone = x,
                       HightestTone,
                       .3f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            finished = false;

            DOTween.To(() => tone,
                       x => tone = x,
                       NormalTone,
                       1.4f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            effect.shadows.overrideState = false;
            effect.midtones.overrideState = false;
            effect.highlights.overrideState = false;

            Destroy(volume.gameObject);
        }

        /// <summary>
        /// Make the player use flash.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player.</param>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="mapSceneLauncher"></param>
        public override IEnumerator UseOutOfBattle(PlayerCharacter playerCharacter,
                                                   MonsterInstance monster,
                                                   ILocalizer localizer,
                                                   MapSceneLauncher mapSceneLauncher)
        {
            if (playerCharacter.CharacterController.CurrentGrid.SceneInfo.Region.IsMoveLockedByBadge(this,
                    out Badge badge))
                if (!playerCharacter.GlobalGameData.HasBadge(badge, playerCharacter.Region))
                {
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/BlockedByBadge",
                                                                 modifiers: new[]
                                                                            {
                                                                                badge.LocalizableName, LocalizableName
                                                                            });

                    yield break;
                }

            if (!playerCharacter.CharacterController.CurrentGrid.SceneInfo.IsDark || playerCharacter.Flash)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            yield return DialogManager.CloseMenus();

            yield return DialogManager.ShowDialogAndWait("Battle/Move/Used",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monster.GetNameOrNickName(localizer),
                                                                        localizer[LocalizableName]
                                                                    });

            yield return playerCharacter.UseFlash();

            playerCharacter.GlobalGridManager.StopAllActors = false;
            playerCharacter.LockInput(false);
        }
    }
}