using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.InBattle
{
    /// <summary>
    /// Data class representing a ball item effect.
    /// This has no catch rate logic, that is stored in the actual ball object.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/InBattle/BallEffect",
                     fileName = "BallEffect")]
    [InfoBox("This has no catch rate logic, that is stored in the actual ball object.")]
    public class BallEffect : UseInBattleItemEffect
    {
        /// <summary>
        /// Check if the item can be used in this moment.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="user">User of the item.</param>
        /// <returns></returns>
        public override bool CanBeUsed(BattleManager battleManager, Battler user) =>
            battleManager.EnemyType == EnemyType.Wild
         && battleManager.Battlers.GetBattlersFighting(BattlerType.Enemy).Count == 1;

        /// <summary>
        /// Call the battle manager and try to catch.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="userType">Battler type of the user.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="battleManager"></param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator Use(Item item,
                                        BattlerType userType,
                                        int userIndex,
                                        BattleManager battleManager,
                                        Action<bool> finished)
        {
            (BattlerType _, int battleIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(battleManager
                                                               .Battlers.GetBattlersFighting(BattlerType
                                                                   .Enemy)
                                                               .First());

            yield return battleManager.Capture.TryCaptureMonster(battleIndex, (Ball)item);

            finished?.Invoke(true);
        }
    }
}