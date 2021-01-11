﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using RoR2.UI.MainMenu;

namespace UmbraMenu.Menus
{
    public sealed class Main : Menu
    {
        private static readonly IMenu main = new NormalMenu(0, new Rect(10, 10, 20, 20), "U M B R A   M E N U");

        public Main() : base(main)
        {
            Id = 0;
            Rect = new Rect(10, 10, 20, 20); // Starting Position

            if (Loader.updateAvailable)
            {
                Title = $"U M B R A \n<color=yellow>O U T D A T E D</color>";
            }
            else if (Loader.upToDate)
            {
                Title = $"U M B R A \n<color=grey>v{UmbraMenu.VERSION}</color>";
            }
            else if (Loader.devBuild)
            {
                Title = $"U M B R A \n<color=yellow>D E V</color>";
            }

            if (UmbraMenu.characterCollected)
            {
                /*
                Button togglePlayer = new Button(new TogglableButton(this, 1, "P L A Y E R : O F F", "P L A Y E R : O N", PlayerButtonAction, PlayerButtonAction));
                Button toggleMovement = new Button(new TogglableButton(this, 2, "M O V E M E N T : O F F", "M O V E M E N T : O N", MovementButtonAction, MovementButtonAction));
                Button toggleItems = new Button(new TogglableButton(this, 3, "I T E M S : O F F", "I T E M S : O N", ItemsButtonAction, ItemsButtonAction));
                Button toggleSpawn = new Button(new TogglableButton(this, 4, "S P A W N : O F F", "S P A W N : O N", SpawnButtonAction, SpawnButtonAction));
                Button toggleTeleporter = new Button(new TogglableButton(this, 5, "T E L E P O R T E R : O F F", "T E L E P O R T E R : O N", TeleporterButtonAction, TeleporterButtonAction));
                Button toggleRender = new Button(new TogglableButton(this, 6, "R E N D E R : O F F", "R E N D E R: O N", RenderButtonAction, RenderButtonAction));
                Button toggleSettings = new Button(new TogglableButton(this, 7, "S E T T I N G S : O F F", "L O B B Y : O N", SettingsButtonAction, SettingsButtonAction));
                */
                Button togglePlayer = new Button(new TogglableButton(this, 1, "P L A Y E R : O F F", "P L A Y E R : O N", PlayerButtonAction, PlayerButtonAction));
                Button toggleMovement = new Button(new TogglableButton(this, 2, "M O V E M E N T : O F F", "M O V E M E N T : O N", null, null));
                Button toggleItems = new Button(new TogglableButton(this, 3, "I T E M S : O F F", "I T E M S : O N", null, null));
                Button toggleSpawn = new Button(new TogglableButton(this, 4, "S P A W N : O F F", "S P A W N : O N", null, null));
                Button toggleTeleporter = new Button(new TogglableButton(this, 5, "T E L E P O R T E R : O F F", "T E L E P O R T E R : O N", null, null));
                Button toggleRender = new Button(new TogglableButton(this, 6, "R E N D E R : O F F", "R E N D E R: O N", null, null));
                Button toggleSettings = new Button(new TogglableButton(this, 7, "S E T T I N G S : O F F", "L O B B Y : O N", null, null));
                Button unloadMenu = new Button(new TogglableButton(this, 8, "U N L O A D   M E N U", "C O N F I R M ?", DoNothing, UnloadMenu));

                Buttons = new List<Button>
                {
                    togglePlayer,
                    toggleMovement,
                    toggleItems,
                    toggleSpawn,
                    toggleTeleporter,
                    toggleRender,
                    toggleSettings,
                    unloadMenu
                };
                NumberOfButtons = Buttons.Count;
            }
            else
            {
                if (Loader.updateAvailable)
                {
                    Button text1 = new Button(new TextButton(this, 2, "<color=yellow>Buttons will be availble in game.</color>"));
                    Button text2 = new Button(new TextButton(this, 3, "<color=#11ccee>Created By Neonix#1337.\n Feel Free to Message me on discord.</color>"));
                    Button text3 = new Button(new TextButton(this, 4, "<color=#11ccee>Download the latest version on my github.\nAcher0ns/Umbra-Mod-Menu</color>"));
                    Buttons = new List<Button>
                    {
                        text1,
                        text2,
                        text3
                    };
                    NumberOfButtons = Buttons.Count;
                }

                if (Loader.upToDate || Loader.devBuild)
                {
                    Button text1 = new Button(new TextButton(this, 2, "<color=yellow>Buttons will be availble in game.</color>"));
                    Button text2 = new Button(new TextButton(this, 3, "<color=#11ccee>Created By Neonix#1337.\n Feel Free to Message me on discord.</color>"));
                    Button text3 = new Button(new TextButton(this, 4, "<color=#11ccee>with bug Reports or suggestions.</color>"));
                    Buttons = new List<Button>
                    {
                        text1,
                        text2,
                        text3
                    };
                    NumberOfButtons = Buttons.Count;
                }
            }
        }

        public override void Draw()
        {
            SetWindow();
            if (Enabled)
            {
                base.Draw();
            }
        }

        private static void PlayerButtonAction() => ToggleMenu(Utility.FindMenuById(1));
        private static void MovementButtonAction() => ToggleMenu(Utility.FindMenuById(2));
        private static void ItemsButtonAction() => ToggleMenu(Utility.FindMenuById(3));
        private static void SpawnButtonAction() => ToggleMenu(Utility.FindMenuById(4));
        private static void TeleporterButtonAction() => ToggleMenu(Utility.FindMenuById(5));
        private static void RenderButtonAction() => ToggleMenu(Utility.FindMenuById(6));
        private static void SettingsButtonAction() => ToggleMenu(Utility.FindMenuById(7));
        private static void UnloadMenu() => Loader.Unload();
        private static void DoNothing() => Utility.StubbedFunction();
        public static void ToggleMenu(Menu menu)
        {
            menu.Enabled = !menu.Enabled;
        }
        
    }
}
