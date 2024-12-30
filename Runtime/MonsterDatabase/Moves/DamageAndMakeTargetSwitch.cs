using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that deal damage and make the target switch.
    /// </summary>
    public abstract class DamageAndMakeTargetSwitch : DamageMove
    {
        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Execute the secondary effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            yield return base.ExecuteSecondaryEffect(battleManager,
                                                     localizer,
                                                     userType,
                                                     userIndex,
                                                     user,
                                                     targets,
                                                     hitNumber,
                                                     expectedHits,
                                                     externalPowerMultiplier,
                                                     ignoresAbilities);

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                bool bypassSubstitute = user.CanUseAbility(battleManager, false)
                                     && user.GetAbility()
                                            .ByPassesSubstitute(targetType,
                                                                targetIndex,
                                                                battleManager,
                                                                userType,
                                                                userIndex,
                                                                this);

                if (!target.CanBattle) yield break;

                if (!target.IsAffectedBySecondaryEffectsOfDamageMove(user,
                                                                     this,
                                                                     LastDamageMade,
                                                                     ignoresAbilities,
                                                                     battleManager))
                    yield break;

                // Used against wilds.
                if (targetType == BattlerType.Enemy && battleManager.EnemyType == EnemyType.Wild)
                {
                    if (battleManager.Battlers.GetBattlersFighting(BattlerType.Enemy).Count > 1)
                        // No extra effect if there is more than one wild.
                        yield break;

                    if (user.StatData.Level >= target.StatData.Level)
                        // Run away if user has more level.
                        yield return battleManager.Battlers.RunAway(userType, userIndex, false, true);
                }
                else if
                    (target.CanSwitch(battleManager,
                                      userType,
                                      userIndex,
                                      this,
                                      ignoresAbilities,
                                      null,
                                      false,
                                      true)
                  && (bypassSubstitute
                   || !target.Substitute
                             .SubstituteEnabled)) // Used against trainers, force switch to the next available if there is.
                {
                    int newBattlerIndex = -1;

                    List<Battler> targetRoster = battleManager.Rosters.GetRoster(targetType, targetIndex);

                    for (int i = 0; i < targetRoster.Count; ++i)
                    {
                        Battler candidate = targetRoster[i];
                        if (!candidate.CanBattle || candidate == target) continue;

                        newBattlerIndex = i;
                        break;
                    }

                    if (newBattlerIndex != -1)
                        yield return battleManager.BattleManagerBattlerSwitch.SwitchBattler(targetType,
                            targetIndex,
                            newBattlerIndex);
                }
            }
        }
    }
}