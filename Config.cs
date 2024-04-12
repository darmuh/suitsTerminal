﻿using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;
using TerminalApi;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> extensiveLogging;
        public static ConfigEntry<int> suitsOnRack;
        public static ConfigEntry<bool> hideRack;
        public static ConfigEntry<bool> hideBoots;
        public static ConfigEntry<bool> keepSuitsWithNegativeIDs;
        public static ConfigEntry<bool> randomSuitCommand;
        public static ConfigEntry<bool> chatCommands;
        public static ConfigEntry<bool> dontRemove;
        public static ConfigEntry<bool> enforcePaidSuits;
        public static ConfigEntry<bool> terminalCommands;
        public static ConfigEntry<bool> advancedTerminalMenu;
        public static ConfigEntry<bool> enablePiPCamera;
        public static ConfigEntry<bool> chatHints;
        public static ConfigEntry<bool> bannerHints;

        public static ConfigEntry<string> menuUp;
        public static ConfigEntry<string> menuDown;
        public static ConfigEntry<string> menuLeft;
        public static ConfigEntry<string> menuRight;
        public static ConfigEntry<string> leaveMenu;
        public static ConfigEntry<string> selectMenu;
        public static ConfigEntry<string> togglePiP;
        public static ConfigEntry<string> togglePiPZoom;
        public static ConfigEntry<string> togglePiPRotation;
        public static ConfigEntry<string> togglePiPHeight;

        public static ConfigEntry<int> setPiPCullingMask;
        

        public static void Settings()
        {

            suitsTerminal.Log.LogInfo("Reading configuration settings");

            //General Configs
            SConfig.suitsOnRack = suitsTerminal.instance.Config.Bind("General", "suitsOnRack", 0, new ConfigDescription("Number of suits to keep on the rack. (Up to 13)", new AcceptableValueRange<int>(0, 13)));
            SConfig.keepSuitsWithNegativeIDs = suitsTerminal.instance.Config.Bind<bool>("General", "keepSuitsWithNegativeIDs", false, "Enable this to attempt to keep suits with negative suitIDs, this option could break the mod or others");
            SConfig.randomSuitCommand = suitsTerminal.instance.Config.Bind<bool>("General", "randomSuitCommand", true, "Enable/Disable the randomsuit terminal command.");
            SConfig.chatCommands = suitsTerminal.instance.Config.Bind<bool>("General", "chatCommands", false, "Enable/Disable suits commands via chat (!suits/!wear).");
            SConfig.dontRemove = suitsTerminal.instance.Config.Bind<bool>("General", "dontRemove", false, "Enable this to stop this mod from removing suits from the rack.");
            SConfig.terminalCommands = suitsTerminal.instance.Config.Bind<bool>("General", "terminalCommands", true, "Enable/Disable the base suits commands via terminal (suits, wear).");
            SConfig.extensiveLogging = suitsTerminal.instance.Config.Bind<bool>("General", "extensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            SConfig.enforcePaidSuits = suitsTerminal.instance.Config.Bind<bool>("General", "enforcePaidSuits", true, "Enable or Disable enforcing paid suits being locked until they are paid for & unlocked.");
            SConfig.hideBoots = suitsTerminal.instance.Config.Bind<bool>("General", "hideBoots", false, "Enable this to hide the boots by the rack.");
            SConfig.hideRack = suitsTerminal.instance.Config.Bind<bool>("General", "hideRack", false, "Enable this to hide the rack, (rack will not be hidden if dontRemove is enabled, suitsOnRack integer will be ignored if rack hidden).");


            //Menu Binds
            SConfig.advancedTerminalMenu = suitsTerminal.instance.Config.Bind<bool>("General", "advancedTerminalMenu", true, "Enable this to utilize the advanced menu system and keybinds below");
            SConfig.menuLeft = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuLeft", "LeftArrow", "Set key to press to go to previous page in advanced menu system");
            SConfig.menuRight = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuRight", "RightArrow", "Set key to press to go to next page in advanced menu system");
            SConfig.menuUp = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuUp", "UpArrow", "Set key to press to go to previous item on page in advanced menu system");
            SConfig.menuDown = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuDown", "DownArrow", "Set key to press to go to next item on page in advanced menu system");
            SConfig.leaveMenu = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "leaveMenu", "Backspace", "Set key to press to leave advanced menu system");
            SConfig.selectMenu = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "selectMenu", "Enter", "Set key to press to select an option in advanced menu system");
            SConfig.togglePiP = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiP", "F12", "Set key to press to toggle PiP (mirror cam) on/off in advanced menu system");
            SConfig.togglePiPZoom = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiPZoom", "Minus", "Set key to press to toggle PiP (mirror cam) zoom in advanced menu system");
            SConfig.togglePiPRotation = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiPRotation", "Equals", "Set key to press to toggle PiP (mirror cam) rotation in advanced menu system");
            SConfig.togglePiPHeight = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiPHeight", "Backslash", "Set key to press to toggle PiP (mirror cam) height in advanced menu system");

            SConfig.enablePiPCamera = suitsTerminal.instance.Config.Bind<bool>("advancedTerminalMenu", "enablePiPCamera", true, "Disable this to stop the PiP camera from being created");
            SConfig.setPiPCullingMask = suitsTerminal.instance.Config.Bind<int>("advancedTerminalMenu", "setPiPCullingMask", 565909343, "Use this configuration option to change the camera's culling mask. (don't change unless you have an idea what you're doing)");

            //Hints
            SConfig.chatHints = suitsTerminal.instance.Config.Bind<bool>("Hints", "chatHints", true, "Disable this to remove the chat hints displayed at load in.");
            SConfig.bannerHints = suitsTerminal.instance.Config.Bind<bool>("Hints", "bannerHints", true, "Disable this to remove the banner hints displayed at load in.");

            //creds to Kittenji
            PropertyInfo orphanedEntriesProp = suitsTerminal.instance.Config.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(suitsTerminal.instance.Config, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            suitsTerminal.instance.Config.Save(); // Save the config file
        }
    }
}
