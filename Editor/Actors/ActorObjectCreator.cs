using System.Linq;
using UnityEditor;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Editor.Actors
{
    /// <summary>
    /// Class in charge of creating template objects for actors.
    /// </summary>
    public class ActorObjectCreator : Loggable<ActorObjectCreator>
    {
        /// <summary>
        /// Reference to the actor prefab library.
        /// </summary>
        private static ActorPrefabLibrary Library
        {
            get
            {
                if (library == null) library = AssetManagementUtils.FindAssetsByType<ActorPrefabLibrary>().First();
                return library;
            }
        }

        /// <summary>
        /// Backfield for Library.
        /// </summary>
        private static ActorPrefabLibrary library;

        /// <summary>
        /// Create a blank character.
        /// </summary>
        [MenuItem("GameObject/YAPU/Blank/Character", false, 10)]
        private static void CreateBlankCharacter() => PrefabUtility.InstantiatePrefab(Library.BlankCharacter);

        /// <summary>
        /// Create an EmptyTrigger.
        /// </summary>
        [MenuItem("GameObject/YAPU/Blank/EmptyTrigger", false, 10)]
        private static void CreateEmptyTrigger() => PrefabUtility.InstantiatePrefab(Library.EmptyTrigger);

        /// <summary>
        /// Create a blank invisible character.
        /// </summary>
        [MenuItem("GameObject/YAPU/Blank/Invisible", false, 10)]
        private static void CreateBlankInvisible() => PrefabUtility.InstantiatePrefab(Library.Invisible);

        /// <summary>
        /// Create a static trainer object.
        /// </summary>
        [MenuItem("GameObject/YAPU/Trainers/Static", false, 10)]
        private static void CreateStaticTrainer() => PrefabUtility.InstantiatePrefab(Library.StaticTrainer);

        /// <summary>
        /// Create a Circle trainer object.
        /// </summary>
        [MenuItem("GameObject/YAPU/Trainers/Circle", false, 10)]
        private static void CreateCircleTrainer() => PrefabUtility.InstantiatePrefab(Library.CircleTrainer);

        /// <summary>
        /// Create a look around trainer object.
        /// </summary>
        [MenuItem("GameObject/YAPU/Trainers/Look around", false, 10)]
        private static void CreateLookAroundTrainer() => PrefabUtility.InstantiatePrefab(Library.LookAroundLoopTrainer);

        /// <summary>
        /// Create a random look around trainer object.
        /// </summary>
        [MenuItem("GameObject/YAPU/Trainers/Random look around", false, 10)]
        private static void CreateRandomLookAroundTrainer() =>
            PrefabUtility.InstantiatePrefab(Library.RandomLookAroundTrainer);

        /// <summary>
        /// Create a random walk trainer object.
        /// </summary>
        [MenuItem("GameObject/YAPU/Trainers/Random walk", false, 10)]
        private static void CreateRandomWalkTrainer() => PrefabUtility.InstantiatePrefab(Library.RandomWalkTrainer);

        /// <summary>
        /// Create a random walk monster
        /// </summary>
        [MenuItem("GameObject/YAPU/Monsters/Random walk", false, 10)]
        private static void CreateRandomWalkMonster() => PrefabUtility.InstantiatePrefab(Library.RandomWalkMonster);

        /// <summary>
        /// Create a pickable item.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Pick up item", false, 10)]
        private static void CreateRandomPickupItem() => PrefabUtility.InstantiatePrefab(Library.PickUpItem);

        /// <summary>
        /// Create an NPC with a simple dialog.
        /// </summary>
        [MenuItem("GameObject/YAPU/NPCs/Simple dialog", false, 10)]
        private static void CreateSimpleDialogNPC() => PrefabUtility.InstantiatePrefab(Library.SimpleDialogNPC);

        /// <summary>
        /// Create an NPC that gives the player an item.
        /// </summary>
        [MenuItem("GameObject/YAPU/NPCs/Give item", false, 10)]
        private static void CreateGiveItemNPC() => PrefabUtility.InstantiatePrefab(Library.GiveItemNPC);

        /// <summary>
        /// Create an NPC that gives the player a monster.
        /// </summary>
        [MenuItem("GameObject/YAPU/NPCs/Give monster", false, 10)]
        private static void CreateGiveMonsterNPC() => PrefabUtility.InstantiatePrefab(Library.GiveMonsterNPC);

        /// <summary>
        /// Create a sign.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Sign", false, 10)]
        private static void CreateSign() => PrefabUtility.InstantiatePrefab(Library.Sign);

        /// <summary>
        /// Create a temporary cuttable tree.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Temporary cuttable tree", false, 10)]
        private static void CreateTemporaryCuttableTree() =>
            PrefabUtility.InstantiatePrefab(Library.TemporaryCuttableTree);

        /// <summary>
        /// Create a permanent cuttable tree.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Permanent cuttable tree", false, 10)]
        private static void CreatePermanentCuttableTree() =>
            PrefabUtility.InstantiatePrefab(Library.PermanentCuttableTree);

        /// <summary>
        /// Create a temporary breakable rock.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Temporary breakable rock", false, 10)]
        private static void CreateTemporaryBreakableRock() =>
            PrefabUtility.InstantiatePrefab(Library.TemporaryBreakableRock);

        /// <summary>
        /// Create a permanent breakable rock.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Permanent breakable rock", false, 10)]
        private static void CreatePermanentBreakableRock() =>
            PrefabUtility.InstantiatePrefab(Library.PermanentBreakableRock);

        /// <summary>
        /// Create aa temporary movable boulder.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Temporary movable boulder", false, 10)]
        private static void CreateTemporaryMovableBoulder() =>
            PrefabUtility.InstantiatePrefab(Library.TemporaryMovableBoulder);

        /// <summary>
        /// Create a temporary removable whirlpool.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Temporary removable whirlpool", false, 10)]
        private static void CreateTemporaryRemovableWhirlpool() =>
            PrefabUtility.InstantiatePrefab(Library.TemporaryRemovableWhirlpool);

        /// <summary>
        /// Create a permanent removable whirlpool.
        /// </summary>
        [MenuItem("GameObject/YAPU/Other/Permanent removable whirlpool", false, 10)]
        private static void CreatePermanentRemovableWhirlpool() =>
            PrefabUtility.InstantiatePrefab(Library.PermanentRemovableWhirlpool);
    }
}