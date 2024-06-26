using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> extensiveLogging { get; internal set; }
        public static ConfigEntry<int> suitsOnRack { get; internal set; }
        public static ConfigEntry<bool> hideRack{ get; internal set; }
        public static ConfigEntry<bool> hideBoots{ get; internal set; }
        public static ConfigEntry<bool> keepSuitsWithNegativeIDs{ get; internal set; }
        public static ConfigEntry<bool> randomSuitCommand{ get; internal set; }
        public static ConfigEntry<bool> chatCommands{ get; internal set; }
        public static ConfigEntry<bool> dontRemove{ get; internal set; }
        public static ConfigEntry<bool> enforcePaidSuits{ get; internal set; }
        public static ConfigEntry<bool> terminalCommands{ get; internal set; }
        public static ConfigEntry<bool> advancedTerminalMenu{ get; internal set; }
        public static ConfigEntry<bool> enablePiPCamera{ get; internal set; }
        public static ConfigEntry<bool> chatHints{ get; internal set; }
        public static ConfigEntry<bool> bannerHints{ get; internal set; }
        public static ConfigEntry<bool> useOpenBodyCams { get; internal set; }
        public static ConfigEntry<string> obcResolution { get; internal set; }


        public static ConfigEntry<string> menuUp{ get; internal set; }
        public static ConfigEntry<string> menuDown{ get; internal set; }
        public static ConfigEntry<string> menuLeft{ get; internal set; }
        public static ConfigEntry<string> menuRight{ get; internal set; }
        public static ConfigEntry<string> leaveMenu{ get; internal set; }
        public static ConfigEntry<string> selectMenu{ get; internal set; }
        public static ConfigEntry<string> helpMenu{ get; internal set; }
        public static ConfigEntry<string> favItemKey{ get; internal set; }
        public static ConfigEntry<string> favMenuKey{ get; internal set; }
        public static ConfigEntry<string> togglePiP{ get; internal set; }
        public static ConfigEntry<string> togglePiPZoom{ get; internal set; }
        public static ConfigEntry<string> togglePiPRotation{ get; internal set; }
        public static ConfigEntry<string> togglePiPHeight{ get; internal set; }

        public static ConfigEntry<string> suitsOnRackList{ get; internal set; }
        public static ConfigEntry<bool> suitsOnRackCustom{ get; internal set; }
        public static ConfigEntry<string> favoritesMenuList{ get; internal set; }
        public static ConfigEntry<string> suitsSortingStyle{ get; internal set; }

        public static ConfigEntry<float> menuKeyPressDelay { get; internal set; }
        public static ConfigEntry<float> menuPostSelectDelay { get; internal set; }


        public static void Settings()
        {

            suitsTerminal.Log.LogInfo("Reading configuration settings");

            //General Configs
            advancedTerminalMenu = MakeBool("General", "advancedTerminalMenu", true, "Enable this to utilize the advanced menu system and keybinds below");
            extensiveLogging = MakeBool("General", "extensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            enforcePaidSuits = MakeBool("General", "enforcePaidSuits", true, "Enable or Disable enforcing paid suits being locked until they are paid for & unlocked.");
            keepSuitsWithNegativeIDs = MakeBool("General", "keepSuitsWithNegativeIDs", false, "Enable this to attempt to keep suits with negative suitIDs, this option could break the mod or others");
            randomSuitCommand = MakeBool("General", "randomSuitCommand", false, "Enable/Disable the randomsuit terminal command.");
            chatCommands = MakeBool("General", "chatCommands", false, "Enable/Disable suits commands via chat (!suits/!wear).");
            terminalCommands = MakeBool("General", "terminalCommands", true, "Enable/Disable the base suits commands via terminal (suits, wear).");
            dontRemove = MakeBool("General", "dontRemove", false, "Enable this to stop this mod from removing suits from the rack and make it compatible with other mods like TooManySuits.");

            //rack config
            suitsOnRack = MakeClampedInt("Rack Settings", "suitsOnRack", 13, "Number of suits to keep on the rack. (Up to 13)", 0, 13);
            suitsOnRackCustom = MakeBool("Rack Settings", "suitsOnRackCustom", false, "Enable this to set specific suits on the rack from the [suitsOnRackList] when [suitsOnRack] is above 0");
            suitsOnRackList = MakeString("Rack Settings", "suitsOnRackList", "YORU, Blue Sapphire, Arctic, OMEN, Elite, GlowRed, GreenImposter, HEVSuit, Medic, RedImposter, Shadow, Santa, Speedster", "when suitsOnRackCustom is enabled, list suits to display on the rack by name (separated by commas & case sensitive). NOTE: If no suits match these names, none will be displayed on the rack.");
            hideBoots = MakeBool("Rack Settings", "hideBoots", false, "Enable this to hide the boots by the rack.");
            hideRack = MakeBool("Rack Settings", "hideRack", false, "Enable this to hide the rack, (rack will not be hidden if dontRemove is enabled, suitsOnRack integer will be ignored if rack hidden).");
            suitsSortingStyle = MakeClampedString("Rack Settings", "suitsSortingStyle", "alphabetical (UnlockableName)", "How suits will be sorted in menus & on the rack", new AcceptableValueList<string>("alphabetical", "numerical", "none"));

            //Menu Binds
            favoritesMenuList = MakeString("advancedTerminalMenu", "favoritesMenuList", "", "Favorited suit names will be stored here and displayed in the advancedTerminalMenu.");
            enablePiPCamera = MakeBool("advancedTerminalMenu", "enablePiPCamera", true, "Disable this to stop the PiP camera from being created");
            menuLeft = MakeString("advancedTerminalMenu", "menuLeft", "LeftArrow", "Set key to press to go to previous page in advanced menu system");
            menuRight = MakeString("advancedTerminalMenu", "menuRight", "RightArrow", "Set key to press to go to next page in advanced menu system");
            menuUp = MakeString("advancedTerminalMenu", "menuUp", "UpArrow", "Set key to press to go to previous item on page in advanced menu system");
            menuDown = MakeString("advancedTerminalMenu", "menuDown", "DownArrow", "Set key to press to go to next item on page in advanced menu system");
            leaveMenu = MakeString("advancedTerminalMenu", "leaveMenu", "Backspace", "Set key to press to leave advanced menu system");
            selectMenu = MakeString("advancedTerminalMenu", "selectMenu", "Enter", "Set key to press to select an item in advanced menu system");
            helpMenu = MakeString("advancedTerminalMenu", "helpMenu", "H", "Set key to press to toggle help & controls page in advanced menu system");
            favItemKey = MakeString("advancedTerminalMenu", "favItemKey", "F", "Set key to press to set an item as a favorite in advanced menu system");
            favMenuKey = MakeString("advancedTerminalMenu", "favMenuKey", "F1", "Set key to press to show favorites menu in advanced menu system");
            togglePiP = MakeString("advancedTerminalMenu", "togglePiP", "F12", "Set key to press to toggle PiP (mirror cam) on/off in advanced menu system");
            togglePiPZoom = MakeString("advancedTerminalMenu", "togglePiPZoom", "Minus", "Set key to press to toggle PiP (mirror cam) zoom in advanced menu system");
            togglePiPRotation = MakeString("advancedTerminalMenu", "togglePiPRotation", "Equals", "Set key to press to toggle PiP (mirror cam) rotation in advanced menu system");
            togglePiPHeight = MakeString("advancedTerminalMenu", "togglePiPHeight", "Backslash", "Set key to press to toggle PiP (mirror cam) height in advanced menu system");
            
            //Hints
            chatHints = MakeBool("Hints", "chatHints", true, "Disable this to remove the chat hints displayed at load in.");
            bannerHints = MakeBool("Hints", "bannerHints", true, "Disable this to remove the banner hints displayed at load in.");

            //OpenBodyCams
            useOpenBodyCams = MakeBool("OpenBodyCams", "useOpenBodyCams", true, "Disable this to remove the banner hints displayed at load in.");
            obcResolution = MakeString("OpenBodyCams", "obcResolution", "1000; 700", "Set the resolution of the Mirror Camera created with OpenBodyCams");

            //Clamped Floats
            menuKeyPressDelay = MakeClampedFloat("advancedTerminalMenu", "menuKeyPressDelay", 0.15f, "Regular delay when checking for key presses in the advancedTerminalMenu. (This delay will be added ontop of menuPostSelectDelay)", 0.05f, 1f);
            menuPostSelectDelay = MakeClampedFloat("advancedTerminalMenu", "menuPostSelectDelay", 0.1f, "Delay used after a key press is registered in the advancedTerminalMenu.", 0.05f, 1f);

            //creds to Kittenji
            PropertyInfo orphanedEntriesProp = suitsTerminal.instance.Config.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(suitsTerminal.instance.Config, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            suitsTerminal.instance.Config.Save(); // Save the config file
        }

        private static ConfigEntry<bool> MakeBool(string section, string configItemName, bool defaultValue, string configDescription)
        {
            return suitsTerminal.instance.Config.Bind<bool>(section, configItemName, defaultValue, configDescription);
        }

        private static ConfigEntry<int> MakeInt(string section, string configItemName, int defaultValue, string configDescription)
        {
            return suitsTerminal.instance.Config.Bind<int>(section, configItemName, defaultValue, configDescription);
        }

        private static ConfigEntry<string> MakeClampedString(string section, string configItemName, string defaultValue, string configDescription, AcceptableValueList<string> acceptedValues)
        {
            return suitsTerminal.instance.Config.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, acceptedValues));
        }

        private static ConfigEntry<int> MakeClampedInt(string section, string configItemName, int defaultValue, string configDescription, int minValue, int maxValue)
        {
            return suitsTerminal.instance.Config.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<int>(minValue, maxValue)));
        }

        private static ConfigEntry<float> MakeClampedFloat(string section, string configItemName, float defaultValue, string configDescription, float minValue, float maxValue)
        {
            return suitsTerminal.instance.Config.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<float>(minValue, maxValue)));
        }

        private static ConfigEntry<string> MakeString(string section, string configItemName, string defaultValue, string configDescription)
        {
            //monitorMessages = Plugin.instance.Config.Bind("Ship Stuff", "monitorMessages", "BEHIND YOU, HAVING FUN?, TAG YOU'RE IT, DANCE FOR ME, IM HIDING, #######, ERROR, DEATH, NO MORE SCRAP", "Comma-separated list of messages the ghostGirl can display on the ship monitors when sending a code.");

            return suitsTerminal.instance.Config.Bind(section, configItemName, defaultValue, configDescription);
        }
    }
}
