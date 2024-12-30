using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing the infatuation status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Infatuation", fileName = "Infatuation")]
    public class Infatuation : VolatileStatus
    {
        /// <summary>
        /// Texture for the particles.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D ParticleTexture;

        /// <summary>
        /// Animation audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Chance of attacking while infatuated.
        /// </summary>
        [FoldoutGroup("Effect")]
        [PropertyRange(0, 1)]
        [SerializeField]
        private float AttackChance = .5f;

        /// <summary>
        /// Dictionary that keeps track of the the monsters infatuated and the position they are infatuated with.
        /// </summary>
        private readonly Dictionary<Battler, Battler> targets = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">[0] target.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            targets[battler] = (Battler) extraData[0];

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        targets[battler]
                                                                           .GetNameOrNickName(battleManager
                                                                               .Localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveTargets">Targets of the move</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToUseAMove(Battler battler,
                                                      Move move,
                                                      BattleManager battleManager,
                                                      List<(BattlerType Type, int Index)> moveTargets,
                                                      Action<bool> finished)
        {
            if (!targets.TryGetValue(battler, out Battler target))
            {
                Logger.Error("This battler is not registered as infatuated!");

                finished.Invoke(true);

                yield break;
            }

            (BattlerType _, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);

            if (targetIndex == -1 || !target.CanBattle)
            {
                Logger.Info("Target left, removing infatuation.");

                battleManager.Statuses.ScheduleRemoveStatus(this, battler);

                finished.Invoke(true);
            }
            else
            {
                (BattlerType battlerType, int battlerIndex) =
                    battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battlerType, battlerIndex);

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                yield return sprite.FXAnimator.PlayBoostRoutine(battleManager.BattleSpeed, false, ParticleTexture);

                yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetNameOrNickName(battleManager
                                                                               .Localizer),
                                                                            target.GetNameOrNickName(battleManager
                                                                               .Localizer)
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                float chance = battleManager.RandomProvider.Value01();

                Logger.Info("Infatuation attack chance: " + AttackChance + ". Roll: " + chance + ".");

                if (chance < AttackChance)
                    finished.Invoke(true);
                else
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/DidntAttack",
                                                                 localizableModifiers: false,
                                                                 modifiers: battler.GetNameOrNickName(battleManager
                                                                    .Localizer),
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    finished.Invoke(false);
                }
            }
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            targets.Remove(battler);

            yield break; // No message.
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            targets.Remove(battler);

            yield return base.OnBattleEnded(battler);
        }
    }
}