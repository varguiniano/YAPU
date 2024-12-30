using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses
{
    /// <summary>
    /// Freeze monster status.
    /// TODO: Stop animation when frozen.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Freeze", fileName = "Freeze")]
    public class Freeze : Status
    {
        /// <summary>
        /// Chance to thaw out each turn.
        /// </summary>
        [FoldoutGroup("Effect")]
        [PropertyRange(0, 1)]
        [SerializeField]
        private float ThawChance = .2f;

        /// <summary>
        /// List of types that will make the monster thaw when hit.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<MonsterType> ThawingDamageMoveTypes;

        /// <summary>
        /// List of moves that will make the monster thaw when hit.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllDamageMoves))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Move> ThawingMoves;

        /// <summary>
        /// List of moves that, when used, will thaw out the user.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Move> BypassingMoves;

        /// <summary>
        /// List of moves that, when used, will thaw out the user if they are of the specified type.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Move, MonsterType> BypassingMovesWhenSpecificType;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Reference to the frozen audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Called when the status is added in battle.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        /// <param name="extraData">Not used.</param>
        public override IEnumerator OnStatusAddedInBattle(Battler battler,
                                                          BattleManager battleManager,
                                                          bool ignoresAbilities,
                                                          bool showMessage = true,
                                                          params object[] extraData)
        {
            yield return PlayAnimation(battler, battleManager);

            yield return base.OnStatusAddedInBattle(battler, battleManager, ignoresAbilities, showMessage);
        }

        /// <summary>
        /// No need to show message.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showMessage">Should a dialog telling the status change be shown?</param>
        public override IEnumerator OnStatusTickInBattle(Battler battler,
                                                         BattleManager battleManager,
                                                         bool showMessage = true)
        {
            yield return base.OnStatusTickInBattle(battler, battleManager, false);
        }

        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToPerformMove(Battler battler,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Action<bool> finished)
        {
            float roll = battleManager.RandomProvider.Value01();

            if (roll <= ThawChance
             || BypassingMoves.Contains(move)
             || (BypassingMovesWhenSpecificType.ContainsKey(move)
              && move.GetMoveTypeInBattle(battler, battleManager) == BypassingMovesWhenSpecificType[move]))
            {
                yield return battleManager.Statuses.RemoveStatus(battler);
                finished?.Invoke(true);
            }
            else
            {
                yield return PlayAnimation(battler, battleManager);

                yield return DialogManager.ShowDialogAndWait(LocalizableStatusTickKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                finished?.Invoke(false);
            }
        }

        /// <summary>
        /// Called after being hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        public override IEnumerator AboutToBeHitByMove(Move move,
                                                       Battler battler,
                                                       Battler user,
                                                       BattleManager battleManager,
                                                       bool didShowUsedMessageNormally)
        {
            if (ThawingMoves.Contains(move)
             || ThawingDamageMoveTypes.Contains(move.GetMoveTypeInBattle(user, battleManager)))
                yield return battleManager.Statuses.RemoveStatus(battler);
        }

        /// <summary>
        /// Play the status animation.
        /// </summary>
        /// <param name="battler">Reference to the monster instance.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator PlayAnimation(Battler battler, BattleManager battleManager)
        {
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(battler);

            BasicSpriteAnimation animation = Instantiate(AnimationPrefab, sprite.Pivot);

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            yield return animation.PlayAnimation(battleManager.BattleSpeed);

            Destroy(animation.gameObject);
        }
    }
}