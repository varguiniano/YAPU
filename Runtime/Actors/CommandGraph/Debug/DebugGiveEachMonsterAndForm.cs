using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Debug
{
    /// <summary>
    /// Give the player one monster of each species and form.
    /// </summary>
    [Serializable]
    public class DebugGiveEachMonsterAndForm : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Level for the monster to be at.
        /// </summary>
        [SerializeField]
        private byte Level = 60;

        /// <summary>
        /// Add the monsters to the storage.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            foreach (MonsterEntry entry in parameterData.MonsterDatabase.GetMonsterEntries())
            {
                foreach (Form form in entry.AvailableForms)
                {
                    if (form.IsCombatForm) continue;

                    DataByFormEntry formData = entry[form];

                    if (formData.HasBinaryGender && formData.HasMaleMaterialOverride)
                    {
                        MonsterInstance newFemaleMonster = new(parameterData.YAPUSettings,
                                                               parameterData.MonsterDatabase,
                                                               entry,
                                                               form,
                                                               Level,
                                                               gender: MonsterGender.Female);

                        newFemaleMonster.RebuildOriginData(parameterData.PlayerCharacter.Scene.Asset,
                                                           OriginData.Type.Caught,
                                                           parameterData
                                                              .PlayerCharacter.CharacterController.GetCharacterData()
                                                              .LocalizableName,
                                                           parameterData.YAPUSettings,
                                                           parameterData.Localizer);

                        newFemaleMonster.CurrentTrainer =
                            parameterData.PlayerCharacter.CharacterController.GetCharacterData().LocalizableName;

                        parameterData.PlayerCharacter.PlayerStorage.AddMonster(newFemaleMonster);

                        parameterData.PlayerCharacter.PlayerDex.RegisterAsCaught(newFemaleMonster, false, false, false);

                        MonsterInstance newMaleMonster = new(parameterData.YAPUSettings,
                                                             parameterData.MonsterDatabase,
                                                             entry,
                                                             form,
                                                             Level,
                                                             gender: MonsterGender.Male);

                        newMaleMonster.RebuildOriginData(parameterData.PlayerCharacter.Scene.Asset,
                                                         OriginData.Type.Caught,
                                                         parameterData
                                                            .PlayerCharacter.CharacterController.GetCharacterData()
                                                            .LocalizableName,
                                                         parameterData.YAPUSettings,
                                                         parameterData.Localizer);

                        newMaleMonster.CurrentTrainer =
                            parameterData.PlayerCharacter.CharacterController.GetCharacterData().LocalizableName;

                        parameterData.PlayerCharacter.PlayerStorage.AddMonster(newMaleMonster);

                        parameterData.PlayerCharacter.PlayerDex.RegisterAsCaught(newMaleMonster, false, false, false);
                    }
                    else // Either gender is fine.
                    {
                        MonsterInstance newMonster = new(parameterData.YAPUSettings,
                                                         parameterData.MonsterDatabase,
                                                         entry,
                                                         form,
                                                         Level);

                        newMonster.RebuildOriginData(parameterData.PlayerCharacter.Scene.Asset,
                                                     OriginData.Type.Caught,
                                                     parameterData
                                                        .PlayerCharacter.CharacterController.GetCharacterData()
                                                        .LocalizableName,
                                                     parameterData.YAPUSettings,
                                                     parameterData.Localizer);

                        newMonster.CurrentTrainer =
                            parameterData.PlayerCharacter.CharacterController.GetCharacterData().LocalizableName;

                        parameterData.PlayerCharacter.PlayerStorage.AddMonster(newMonster);

                        parameterData.PlayerCharacter.PlayerDex.RegisterAsCaught(newMonster, false, false, false);
                    }

                    if (!form.HasShinyVersion || form.ShinyVersion.IsCombatForm) continue;

                    DataByFormEntry shinyFormData = entry[form.ShinyVersion];

                    if (shinyFormData.HasBinaryGender && shinyFormData.HasMaleMaterialOverride)
                    {
                        MonsterInstance newFemaleShinny = new(parameterData.YAPUSettings,
                                                              parameterData.MonsterDatabase,
                                                              entry,
                                                              form.ShinyVersion,
                                                              Level,
                                                              gender: MonsterGender.Female);

                        newFemaleShinny.RebuildOriginData(parameterData.PlayerCharacter.Scene.Asset,
                                                          OriginData.Type.Caught,
                                                          parameterData
                                                             .PlayerCharacter.CharacterController.GetCharacterData()
                                                             .LocalizableName,
                                                          parameterData.YAPUSettings,
                                                          parameterData.Localizer);

                        newFemaleShinny.CurrentTrainer =
                            parameterData.PlayerCharacter.CharacterController.GetCharacterData().LocalizableName;

                        parameterData.PlayerCharacter.PlayerStorage.AddMonster(newFemaleShinny);

                        parameterData.PlayerCharacter.PlayerDex.RegisterAsCaught(newFemaleShinny, false, false, false);

                        MonsterInstance newMaleShinny = new(parameterData.YAPUSettings,
                                                            parameterData.MonsterDatabase,
                                                            entry,
                                                            form.ShinyVersion,
                                                            Level,
                                                            gender: MonsterGender.Male);

                        newMaleShinny.RebuildOriginData(parameterData.PlayerCharacter.Scene.Asset,
                                                        OriginData.Type.Caught,
                                                        parameterData
                                                           .PlayerCharacter.CharacterController.GetCharacterData()
                                                           .LocalizableName,
                                                        parameterData.YAPUSettings,
                                                        parameterData.Localizer);

                        newMaleShinny.CurrentTrainer =
                            parameterData.PlayerCharacter.CharacterController.GetCharacterData().LocalizableName;

                        parameterData.PlayerCharacter.PlayerStorage.AddMonster(newMaleShinny);

                        parameterData.PlayerCharacter.PlayerDex.RegisterAsCaught(newMaleShinny, false, false, false);
                    }
                    else // Either gender is fine.
                    {
                        MonsterInstance newShinny = new(parameterData.YAPUSettings,
                                                        parameterData.MonsterDatabase,
                                                        entry,
                                                        form.ShinyVersion,
                                                        Level);

                        newShinny.RebuildOriginData(parameterData.PlayerCharacter.Scene.Asset,
                                                    OriginData.Type.Caught,
                                                    parameterData.PlayerCharacter.CharacterController.GetCharacterData()
                                                                 .LocalizableName,
                                                    parameterData.YAPUSettings,
                                                    parameterData.Localizer);

                        newShinny.CurrentTrainer =
                            parameterData.PlayerCharacter.CharacterController.GetCharacterData().LocalizableName;

                        parameterData.PlayerCharacter.PlayerStorage.AddMonster(newShinny);

                        parameterData.PlayerCharacter.PlayerDex.RegisterAsCaught(newShinny, false, false, false);
                    }
                }
            }

            yield break;
        }
    }
}