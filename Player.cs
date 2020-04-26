using System;
using System.Linq;
using UnityEngine;
using RoR2;

namespace UmbraRoR
{
    public class PlayerMod
    {
        public static int damagePerLvl = 10;
        public static int CritPerLvl = 1;
        public static float attackSpeed = 1;
        public static float armor = 0;
        public static float movespeed = 7;
        public static int jumpCount = 1;
        public static ulong xpToGive = 50;
        public static uint moneyToGive = 50;
        public static uint coinsToGive = 50;

        public static void GiveBuff(GUIStyle buttonStyle, string buttonName)
        {
            //LocalPlayerBody.AddBuff(buff);

            int buttonPlacement = 1;
            foreach (string buffName in Enum.GetNames(typeof(BuffIndex)))
            {
                //bool unreleasedullItem = unreleasedItems.Any(item.Contains);
                if (GUI.Button(btn.BtnRect(buttonPlacement, false, buttonName), buffName, buttonStyle))
                {
                    BuffIndex buffIndex = (BuffIndex)Enum.Parse(typeof(BuffIndex), buffName);
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    if (localUser.cachedMasterController && localUser.cachedMasterController.master)
                    {
                        Main.LocalPlayerBody.AddBuff(buffIndex);
                    }
                }
                buttonPlacement++;
            }
        }

        public static void RemoveAllBuffs()
        {
            foreach (string buffName in Enum.GetNames(typeof(BuffIndex)))
            {
                BuffIndex buffIndex = (BuffIndex)Enum.Parse(typeof(BuffIndex), buffName);
                while (Main.LocalPlayerBody.HasBuff(buffIndex))
                {
                    Main.LocalPlayerBody.RemoveBuff(buffIndex);
                }
            }
        }

        // self explanatory
        public static void giveXP()
        {
            Main.LocalPlayer.GiveExperience(xpToGive);
        }
        public static void GiveMoney()
        {
            Main.LocalPlayer.GiveMoney(moneyToGive);
        }
        //uh, duh.
        public static void GiveLunarCoins()
        {
            Main.LocalNetworkUser.AwardLunarCoins(coinsToGive);
        }
        public static void LevelPlayersCrit()
        {
            try
            {
                Main.LocalPlayerBody.levelCrit = (float)CritPerLvl;
            }
            catch (NullReferenceException)
            {
            }
        }

        public static void LevelPlayersDamage()
        {
            try
            {
                Main.LocalPlayerBody.levelDamage = (float)damagePerLvl;
            }
            catch (NullReferenceException)
            {
            }
        }
        public static void SetplayersAttackSpeed()
        {
            try
            {
                Main.LocalPlayerBody.baseAttackSpeed = attackSpeed;
            }
            catch (NullReferenceException)
            {
            }
        }
        public static void SetplayersArmor()
        {
            try
            {
                Main.LocalPlayerBody.baseArmor = armor;
            }
            catch (NullReferenceException)
            {
            }
        }
        public static void SetplayersMoveSpeed()
        {
            try
            {
                Main.LocalPlayerBody.baseMoveSpeed = movespeed;
            }
            catch (NullReferenceException)
            {
            }
        }

        public static void AimBot()
        {
            if (Utils.CursorIsVisible())
            {
                return;
            }
            var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
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

            var inputBank = body.GetComponent<RoR2.InputBankTest>();
            var aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
            var bullseyeSearch = new RoR2.BullseyeSearch();
            var team = body.GetComponent<RoR2.TeamComponent>();
            bullseyeSearch.teamMaskFilter = RoR2.TeamMask.all;
            bullseyeSearch.teamMaskFilter.RemoveTeam(team.teamIndex);
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.sortMode = RoR2.BullseyeSearch.SortMode.Distance;
            bullseyeSearch.maxDistanceFilter = float.MaxValue;
            bullseyeSearch.maxAngleFilter = 20f;// ;// float.MaxValue;
            bullseyeSearch.RefreshCandidates();
            var hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
            if (hurtBox)
            {
                Vector3 direction = hurtBox.transform.position - aimRay.origin;
                inputBank.aimDirection = direction;
            }
        }

        public static void AlwaysSprint()
        {
            var isMoving = Main.LocalNetworkUser.inputPlayer.GetAxis("MoveVertical") != 0f || Main.LocalNetworkUser.inputPlayer.GetAxis("MoveHorizontal") != 0f;
            var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
            if (localUser == null || localUser.cachedMasterController == null || localUser.cachedMasterController.master == null) return;
            var controller = localUser.cachedMasterController;
            var body = controller.master.GetBody();
            if (body && !body.isSprinting && !localUser.inputPlayer.GetButton("Sprint"))
            {
                if (isMoving)
                {
                    body.isSprinting = true;
                }
            }
        }

        //Respawn... Not sure how to implement it.
        public static void Respawn()
        {
            Main.LocalPlayer.RespawnExtraLife();
        }

        public static void GodMode()
        {
            Main.LocalHealth.godMode = true;
        }

        public static void Flight()
        {
            try
            {
                var forwardDirection = Main.LocalPlayerBody.GetComponent<InputBankTest>().moveVector.normalized;
                var aimDirection = Main.LocalPlayerBody.GetComponent<InputBankTest>().aimDirection.normalized;
                var upDirection = Main.LocalPlayerBody.GetComponent<InputBankTest>().moveVector.y + 1;
                var downDirection = Main.LocalPlayerBody.GetComponent<InputBankTest>().moveVector.y - 1;
                var isForward = Vector3.Dot(forwardDirection, aimDirection) > 0f;

                var isSprinting = Main.LocalNetworkUser.inputPlayer.GetButton("Sprint");
                var isJumping = Main.LocalNetworkUser.inputPlayer.GetButton("Jump");
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                var isStrafing = Main.LocalNetworkUser.inputPlayer.GetAxis("MoveVertical") != 0f;

                if (isSprinting)
                {
                    Main.LocalPlayerBody.characterMotor.velocity = forwardDirection * 100f;
                    Main.LocalPlayerBody.characterMotor.velocity.y = upDirection * 0.510005f;
                    if (isStrafing)
                    {
                        if (isForward)
                        {
                            Main.LocalPlayerBody.characterMotor.velocity.y = aimDirection.y * 100f;
                        }
                        else
                        {
                            Main.LocalPlayerBody.characterMotor.velocity.y = aimDirection.y * -100f;
                        }
                    }
                }
                else if (isJumping)
                {
                    Main.LocalPlayerBody.characterMotor.velocity.y = upDirection * 100;
                }
                else
                {
                    Main.LocalPlayerBody.characterMotor.velocity = forwardDirection * 50;
                    Main.LocalPlayerBody.characterMotor.velocity.y = upDirection * 0.510005f;
                    if (isStrafing)
                    {
                        if (isForward)
                        {
                            Main.LocalPlayerBody.characterMotor.velocity.y = aimDirection.y * 50;
                        }
                        else
                        {
                            Main.LocalPlayerBody.characterMotor.velocity.y = aimDirection.y * -50;
                        }
                    }
                }
            }
            catch (NullReferenceException) { }
        }

        public static void UnlockAll()
        {
            //Goes through resource file containing all unlockables... Easily updatable, just paste "RoR2.UnlockCatalog" and GetAllUnlockable does the rest.
            //This is needed to unlock logs
            foreach (var unlockableName in Main.unlockableNames)
            {
                var unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
                NetworkUser networkUser = Util.LookUpBodyNetworkUser(Main.LocalPlayerBody);
                if (networkUser)
                {
                    networkUser.ServerHandleUnlock(unlockableDef);
                }
            }

            //Gives all achievements.
            var achievementManager = AchievementManager.GetUserAchievementManager(LocalUserManager.GetFirstLocalUser());
            foreach (var achievement in AchievementManager.allAchievementDefs)
            {
                achievementManager.GrantAchievement(achievement);
            }

            //Give all survivors
            var profile = LocalUserManager.GetFirstLocalUser().userProfile;
            foreach (var survivor in SurvivorCatalog.allSurvivorDefs)
            {
                if (profile.statSheet.GetStatValueDouble(RoR2.Stats.PerBodyStatDef.totalTimeAlive, survivor.bodyPrefab.name) == 0.0)
                    profile.statSheet.SetStatValueFromString(RoR2.Stats.PerBodyStatDef.totalTimeAlive.FindStatDef(survivor.bodyPrefab.name), "0.1");
            }

            //All items and equipments
            foreach (string itemName in Enum.GetNames(typeof(ItemIndex)))
            {
                ItemIndex itemIndex = (ItemIndex)Enum.Parse(typeof(ItemIndex), itemName);
                profile.DiscoverPickup(PickupCatalog.FindPickupIndex(itemIndex));
            }

            foreach (string equipmentName in Enum.GetNames(typeof(EquipmentIndex)))
            {
                EquipmentIndex equipmentIndex = (EquipmentIndex)Enum.Parse(typeof(EquipmentIndex), equipmentName);
                profile.DiscoverPickup(PickupCatalog.FindPickupIndex(equipmentIndex));
            }
        }
    }
}