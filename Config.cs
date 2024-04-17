using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;
using TerminalApi;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        internal static ConfigEntry<bool> extensiveLogging;
        internal static ConfigEntry<int> suitsOnRack;
        internal static ConfigEntry<bool> hideRack;
        internal static ConfigEntry<bool> hideBoots;
        internal static ConfigEntry<bool> keepSuitsWithNegativeIDs;
        internal static ConfigEntry<bool> randomSuitCommand;
        internal static ConfigEntry<bool> chatCommands;
        internal static ConfigEntry<bool> dontRemove;
        internal static ConfigEntry<bool> enforcePaidSuits;
        internal static ConfigEntry<bool> terminalCommands;
        internal static ConfigEntry<bool> advancedTerminalMenu;
        internal static ConfigEntry<bool> enablePiPCamera;
        internal static ConfigEntry<bool> chatHints;
        internal static ConfigEntry<bool> bannerHints;

        internal static ConfigEntry<string> menuUp;
        internal static ConfigEntry<string> menuDown;
        internal static ConfigEntry<string> menuLeft;
        internal static ConfigEntry<string> menuRight;
        internal static ConfigEntry<string> leaveMenu;
        internal static ConfigEntry<string> selectMenu;
        internal static ConfigEntry<string> favItemKey;
        internal static ConfigEntry<string> favMenuKey;
        internal static ConfigEntry<string> togglePiP;
        internal static ConfigEntry<string> togglePiPZoom;
        internal static ConfigEntry<string> togglePiPRotation;
        internal static ConfigEntry<string> togglePiPHeight;

        internal static ConfigEntry<int> setPiPCullingMask;

        internal static ConfigEntry<string> suitsOnRackList;
        internal static ConfigEntry<bool> suitsOnRackCustom;
        internal static ConfigEntry<string> favoritesMenuList;
        internal static ConfigEntry<string> suitsSortingStyle;


        public static void Settings()
        {

            suitsTerminal.Log.LogInfo("Reading configuration settings");

            //General Configs
            advancedTerminalMenu = suitsTerminal.instance.Config.Bind<bool>("General", "advancedTerminalMenu", true, "Enable this to utilize the advanced menu system and keybinds below");
            extensiveLogging = suitsTerminal.instance.Config.Bind<bool>("General", "extensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            enforcePaidSuits = suitsTerminal.instance.Config.Bind<bool>("General", "enforcePaidSuits", true, "Enable or Disable enforcing paid suits being locked until they are paid for & unlocked.");
            keepSuitsWithNegativeIDs = suitsTerminal.instance.Config.Bind<bool>("General", "keepSuitsWithNegativeIDs", false, "Enable this to attempt to keep suits with negative suitIDs, this option could break the mod or others");
            randomSuitCommand = suitsTerminal.instance.Config.Bind<bool>("General", "randomSuitCommand", false, "Enable/Disable the randomsuit terminal command.");
            chatCommands = suitsTerminal.instance.Config.Bind<bool>("General", "chatCommands", false, "Enable/Disable suits commands via chat (!suits/!wear).");
            terminalCommands = suitsTerminal.instance.Config.Bind<bool>("General", "terminalCommands", true, "Enable/Disable the base suits commands via terminal (suits, wear).");
            dontRemove = suitsTerminal.instance.Config.Bind<bool>("General", "dontRemove", false, "Enable this to stop this mod from removing suits from the rack and make it compatible with other mods like TooManySuits.");

            //rack config
            suitsOnRack = suitsTerminal.instance.Config.Bind("Rack Settings", "suitsOnRack", 13, new ConfigDescription("Number of suits to keep on the rack. (Up to 13)", new AcceptableValueRange<int>(0, 13)));
            suitsOnRackCustom = suitsTerminal.instance.Config.Bind<bool>("Rack Settings", "suitsOnRackCustom", false, "Enable this to set specific suits on the rack from the [suitsOnRackList] when [suitsOnRack] is above 0");
            suitsOnRackList = suitsTerminal.instance.Config.Bind("Rack Settings", "suitsOnRackList", "YORU, Blue Sapphire, Arctic, OMEN, Elite, GlowRed, GreenImposter, HEVSuit, Medic, RedImposter, Shadow, Santa, Speedster", "when suitsOnRackCustom is enabled, list suits to display on the rack by name (separated by commas & case sensitive). NOTE: If no suits match these names, none will be displayed on the rack.");
            hideBoots = suitsTerminal.instance.Config.Bind<bool>("Rack Settings", "hideBoots", false, "Enable this to hide the boots by the rack.");
            hideRack = suitsTerminal.instance.Config.Bind<bool>("Rack Settings", "hideRack", false, "Enable this to hide the rack, (rack will not be hidden if dontRemove is enabled, suitsOnRack integer will be ignored if rack hidden).");
            suitsSortingStyle = suitsTerminal.instance.Config.Bind("Rack Settings", "suitsSortingStyle", "alphabetical (UnlockableName)", new ConfigDescription("How suits will be sorted in menus & on the rack", new AcceptableValueList<string>("alphabetical", "numerical", "none")));

            //Menu Binds
            favoritesMenuList = suitsTerminal.instance.Config.Bind("advancedTerminalMenu", "favoritesMenuList", "", "Favorited suit names will be stored here and displayed in the advancedTerminalMenu.");
            enablePiPCamera = suitsTerminal.instance.Config.Bind<bool>("advancedTerminalMenu", "enablePiPCamera", true, "Disable this to stop the PiP camera from being created");
            setPiPCullingMask = suitsTerminal.instance.Config.Bind<int>("advancedTerminalMenu", "setPiPCullingMask", 565909343, "Use this configuration option to change the camera's culling mask. (don't change unless you have an idea what you're doing)");
            menuLeft = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuLeft", "LeftArrow", "Set key to press to go to previous page in advanced menu system");
            menuRight = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuRight", "RightArrow", "Set key to press to go to next page in advanced menu system");
            menuUp = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuUp", "UpArrow", "Set key to press to go to previous item on page in advanced menu system");
            menuDown = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "menuDown", "DownArrow", "Set key to press to go to next item on page in advanced menu system");
            leaveMenu = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "leaveMenu", "Backspace", "Set key to press to leave advanced menu system");
            selectMenu = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "selectMenu", "Enter", "Set key to press to select an item in advanced menu system");
            favItemKey = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "favItemKey", "F", "Set key to press to set an item as a favorite in advanced menu system");
            favMenuKey = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "favMenuKey", "F1", "Set key to press to show favorites menu in advanced menu system");
            togglePiP = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiP", "F12", "Set key to press to toggle PiP (mirror cam) on/off in advanced menu system");
            togglePiPZoom = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiPZoom", "Minus", "Set key to press to toggle PiP (mirror cam) zoom in advanced menu system");
            togglePiPRotation = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiPRotation", "Equals", "Set key to press to toggle PiP (mirror cam) rotation in advanced menu system");
            togglePiPHeight = suitsTerminal.instance.Config.Bind<string>("advancedTerminalMenu", "togglePiPHeight", "Backslash", "Set key to press to toggle PiP (mirror cam) height in advanced menu system");
            
            //Hints
            chatHints = suitsTerminal.instance.Config.Bind<bool>("Hints", "chatHints", true, "Disable this to remove the chat hints displayed at load in.");
            bannerHints = suitsTerminal.instance.Config.Bind<bool>("Hints", "bannerHints", true, "Disable this to remove the banner hints displayed at load in.");

            //creds to Kittenji
            PropertyInfo orphanedEntriesProp = suitsTerminal.instance.Config.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(suitsTerminal.instance.Config, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            suitsTerminal.instance.Config.Save(); // Save the config file
        }
    }
}
