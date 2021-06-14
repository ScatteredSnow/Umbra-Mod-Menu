﻿using UnityEngine;
using RoR2;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UmbraMenu.Menus
{
    public class Spawn : NormalMenu
    {
        public static TeamIndex[] team = { TeamIndex.Monster, TeamIndex.Neutral, TeamIndex.Player, TeamIndex.None };
        public static int TeamIndexInt = 0;
        public static float MinDistance = 3f;
        public static float MaxDistance = 40f;

        public static List<GameObject> spawnedObjects = new List<GameObject>();

        public MulButton changeMinDistance;
        public MulButton changeMaxDistance;
        public MulButton changeTeamIndex;
        public TogglableButton toggleSpawnListMenu;
        public NormalButton killAll;
        public NormalButton destroyInteractables;

        public Spawn() : base(4, 0, new Rect(738, 515, 20, 20), "SPAWN MENU")
        {

            void DoNothing() => Utility.StubbedFunction();

            changeMinDistance = new MulButton(this, 1, $"MIN DISTANCE : {MinDistance}", DoNothing, IncreaseMinDistance, DecreaseMinDistance);
            changeMaxDistance = new MulButton(this, 2, $"MAX DISTANCE : {MaxDistance}", DoNothing, IncreaseMaxDistance, DecreaseMaxDistance);
            changeTeamIndex = new MulButton(this, 3, $"TEAM : {team[TeamIndexInt]}", DoNothing, IncreaseTeamIndex, DecreaseTeamIndex);
            toggleSpawnListMenu = new TogglableButton(this, 4, "SPAWN LIST : OFF", "SPAWN LIST : ON", ToggleSpawnListMenu, ToggleSpawnListMenu);
            killAll = new NormalButton(this, 5, "KILL ALL", KillAllMobs);
            destroyInteractables = new NormalButton(this, 6, "DESTROY INTERACTABLES", DestroySpawnedInteractables);

            changeMinDistance.MulChange += UpdateMinDist;
            changeMaxDistance.MulChange += UpdateMaxDist;
            changeTeamIndex.MulChange += UpdateTeamIndex;

            AddButtons(new List<Button>()
            {
                changeMinDistance,
                changeMaxDistance,
                changeTeamIndex,
                toggleSpawnListMenu,
                killAll,
                destroyInteractables
            });
            ActivatingButton = UmbraMenu.mainMenu.toggleSpawn;
        }

        public override void Draw()
        {
            if (IsEnabled())
            {
                SetWindow();
                base.Draw();
            }
        }

        public override void Reset()
        {
            TeamIndexInt = 0;
            MinDistance = 3f;
            MaxDistance = 40f;
            base.Reset();
        }

        public void UpdateMinDist(object sender, EventArgs e)
        {
            changeMinDistance.SetText($"MIN DISTANCE : {MinDistance}");
        }

        public void UpdateMaxDist(object sender, EventArgs e)
        {
            changeMaxDistance.SetText($"MAX DISTANCE : {MaxDistance}");
        }

        public void UpdateTeamIndex(object sender, EventArgs e)
        {
            changeTeamIndex.SetText($"TEAM : {team[TeamIndexInt]}");
        }

        private void ToggleSpawnListMenu()
        {
            UmbraMenu.menus[15].ToggleMenu();
        }

        public static void SpawnMob(GUIStyle buttonStyle, string buttonId)
        {
            int buttonPlacement = 1;
            foreach (var spawnCard in UmbraMenu.spawnCards)
            {
                string cardName = spawnCard.ToString();
                string category = "";
                string buttonText = "";
                if (cardName.Contains("MultiCharacterSpawnCard"))
                {
                    cardName = cardName.Replace(" (RoR2.MultiCharacterSpawnCard)", "");
                    category = "CharacterSpawnCard";
                    buttonText = cardName.Replace("csc", "");
                }
                else if (cardName.Contains("CharacterSpawnCard"))
                {
                    cardName = cardName.Replace(" (RoR2.CharacterSpawnCard)", "");
                    category = "CharacterSpawnCard";
                    buttonText = cardName.Replace("csc", "");
                }
                else if (cardName.Contains("InteractableSpawnCard"))
                {
                    cardName = cardName.Replace(" (RoR2.InteractableSpawnCard)", "");
                    category = "InteractableSpawnCard";
                    buttonText = cardName.Replace("isc", "");
                }
                else if (cardName.Contains("BodySpawnCard"))
                {
                    cardName = cardName.Replace(" (RoR2.BodySpawnCard)", "");
                    category = "BodySpawnCard";
                    buttonText = cardName.Replace("bsc", "");
                }
                string path = $"SpawnCards/{category}/{cardName}";
                buttonPlacement++;
            }
        }

        public static void KillAllMobs()
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            var controller = localUser.cachedMasterController;
            if (!controller)
            {
                return;
            }
            var body = controller.master.GetBody();
            if (!body)
            {
                return;
            }

            var bullseyeSearch = new BullseyeSearch
            {
                filterByLoS = false,
                maxDistanceFilter = float.MaxValue,
                maxAngleFilter = float.MaxValue
            };

            List<string> survivor_names = new List<string>();
            foreach (SurvivorDef def in SurvivorCatalog.allSurvivorDefs)
            {
                survivor_names.Add(def.cachedName);
            }


            bullseyeSearch.RefreshCandidates();
            var hurtBoxList = bullseyeSearch.GetResults();
            foreach (var hurtbox in hurtBoxList)
            {

                var mob = HurtBox.FindEntityObject(hurtbox);
                string mobName = mob.name.Replace("Body(Clone)", "");
  
                if (survivor_names.Contains(mobName))
                {
                    continue;
                }
                else
                {
                    var health = mob.GetComponent<HealthComponent>();
                    health.Suicide();
                    Chat.AddMessage($"<color=yellow>Killed {mobName} </color>");
                }

            }
        }

        public void DestroySpawnedInteractables()
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            var controller = localUser.cachedMasterController;
            if (!controller)
            {
                return;
            }
            var body = controller.master.GetBody();
            if (!body)
            {
                return;
            }

            if (spawnedObjects != null)
            {
                foreach (var gameObject in spawnedObjects)
                {
                    UnityEngine.Object.Destroy(gameObject);
                    Chat.AddMessage($"<color=yellow>Destroyed {gameObject.name.Replace("(Clone)", "")} </color>");
                }
                spawnedObjects = new List<GameObject>();
            }
        }

        #region Increase/Decrease Value Actions
        public void IncreaseMinDistance()
        {
            if (MinDistance < MaxDistance)
            {
                MinDistance += 1;
            }
        }

        public void IncreaseMaxDistance()
        {
            if (MaxDistance >= MinDistance)
            {
                MaxDistance += 1;
            }
        }
        public void IncreaseTeamIndex()
        {
            if (TeamIndexInt < 3)
            {
                TeamIndexInt += 1;
            }
            else if (TeamIndexInt == 3)
            {
                TeamIndexInt = 0;
            }
        }

        public void DecreaseMinDistance()
        {
            if (MinDistance > 0)
            {
                MinDistance -= 1;
            }
        }

        public void DecreaseMaxDistance()
        {
            if (MaxDistance > MinDistance)
            {
                MaxDistance -= 1;
            }
        }

        public void DecreaseTeamIndex()
        {
            if (TeamIndexInt > 0)
            {
                TeamIndexInt -= 1;
            }
            else if (TeamIndexInt == 0)
            {
                TeamIndexInt = 3;
            }
        }
        #endregion
    }
}
