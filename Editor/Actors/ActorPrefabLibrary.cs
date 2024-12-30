using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Editor.Actors
{
    /// <summary>
    /// Library that stores prefabs for template actors.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Actors/PrefabLibrary", fileName = "ActorPrefabLibrary")]
    public class ActorPrefabLibrary : WhateverScriptable<ActorPrefabLibrary>
    {
        /// <summary>
        /// Template for a blank character.
        /// </summary>
        public GameObject BlankCharacter;

        /// <summary>
        /// Template for an empty trigger.
        /// </summary>
        public GameObject EmptyTrigger;

        /// <summary>
        /// Template for a blank invisible character.
        /// </summary>
        public GameObject Invisible;

        /// <summary>
        /// Template for static trainers.
        /// </summary>
        public GameObject StaticTrainer;

        /// <summary>
        /// Template for circle loop trainers.
        /// </summary>
        public GameObject CircleTrainer;

        /// <summary>
        /// Template for look around trainers.
        /// </summary>
        public GameObject LookAroundLoopTrainer;

        /// <summary>
        /// Template for random look around trainers.
        /// </summary>
        public GameObject RandomLookAroundTrainer;

        /// <summary>
        /// Template for random walk trainers.
        /// </summary>
        public GameObject RandomWalkTrainer;

        /// <summary>
        /// Template for random walk monsters.
        /// </summary>
        public GameObject RandomWalkMonster;

        /// <summary>
        /// Template for pickup items.
        /// </summary>
        public GameObject PickUpItem;

        /// <summary>
        /// Template for an NPC that just dialogs with the player.
        /// </summary>
        public GameObject SimpleDialogNPC;

        /// <summary>
        /// Template for an NPC gives an item.
        /// </summary>
        public GameObject GiveItemNPC;

        /// <summary>
        /// Template for an NPC gives an monster.
        /// </summary>
        public GameObject GiveMonsterNPC;
        
        /// <summary>
        /// Prefab for a sign.
        /// </summary>
        public GameObject Sign;

        /// <summary>
        /// Prefab for a cuttable tree that doesn't persist between scenes.
        /// </summary>
        public GameObject TemporaryCuttableTree;

        /// <summary>
        /// Prefab for a cuttable tree that persists between scenes.
        /// </summary>
        public GameObject PermanentCuttableTree;

        /// <summary>
        /// Prefab for a BreakableRock that doesn't persist between scenes.
        /// </summary>
        public GameObject TemporaryBreakableRock;

        /// <summary>
        /// Prefab for a BreakableRock that persists between scenes.
        /// </summary>
        public GameObject PermanentBreakableRock;

        /// <summary>
        /// Prefab for a MovableBoulder that doesn't persist between scenes.
        /// </summary>
        public GameObject TemporaryMovableBoulder;

        /// <summary>
        /// Prefab for a RemovableWhirlpool that doesn't persist between scenes.
        /// </summary>
        public GameObject TemporaryRemovableWhirlpool;

        /// <summary>
        /// Prefab for a RemovableWhirlpool that persists between scenes.
        /// </summary>
        public GameObject PermanentRemovableWhirlpool;
    }
}