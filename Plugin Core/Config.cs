using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> ExtensiveLogging { get; internal set; }
        public static ConfigEntry<int> SuitsOnRack { get; internal set; }
        public static ConfigEntry<bool> HideRack{ get; internal set; }
        public static ConfigEntry<bool> HideBoots{ get; internal set; }
        public static ConfigEntry<bool> KeepSuitsWithNegativeIDs{ get; internal set; }
        public static ConfigEntry<bool> RandomSuitCommand{ get; internal set; }
        public static ConfigEntry<bool> ChatCommands{ get; internal set; }
        public static ConfigEntry<bool> DontRemove{ get; internal set; }
        public static ConfigEntry<bool> EnforcePaidSuits{ get; internal set; }
        public static ConfigEntry<bool> TerminalCommands{ get; internal set; }
        public static ConfigEntry<bool> AdvancedTerminalMenu{ get; internal set; }
        public static ConfigEntry<bool> EnablePiPCamera{ get; internal set; }
        public static ConfigEntry<bool> ChatHints{ get; internal set; }
        public static ConfigEntry<bool> BannerHints{ get; internal set; }
        public static ConfigEntry<bool> UseOpenBodyCams { get; internal set; }
        public static ConfigEntry<string> ObcResolution { get; internal set; }
        public static ConfigEntry<string> DefaultSuit {  get; internal set; }


        public static ConfigEntry<string> MenuUp{ get; internal set; }
        public static ConfigEntry<string> MenuDown{ get; internal set; }
        public static ConfigEntry<string> MenuLeft{ get; internal set; }
        public static ConfigEntry<string> MenuRight{ get; internal set; }
        public static ConfigEntry<string> LeaveMenu{ get; internal set; }
        public static ConfigEntry<string> SelectMenu{ get; internal set; }
        public static ConfigEntry<string> HelpMenu{ get; internal set; }
        public static ConfigEntry<string> FavItemKey{ get; internal set; }
        public static ConfigEntry<string> FavMenuKey{ get; internal set; }
        public static ConfigEntry<string> TogglePiP{ get; internal set; }
        public static ConfigEntry<string> TogglePiPZoom{ get; internal set; }
        public static ConfigEntry<string> TogglePiPRotation{ get; internal set; }
        public static ConfigEntry<string> TogglePiPHeight{ get; internal set; }

        public static ConfigEntry<string> SuitsOnRackOnly{ get; internal set; }
        public static ConfigEntry<string> DontAddToRack { get; internal set; }
        public static ConfigEntry<string> DontAddToTerminal { get; internal set; }
        public static ConfigEntry<string> FavoritesMenuList{ get; internal set; }
        public static ConfigEntry<string> SuitsSortingStyle{ get; internal set; }

        public static ConfigEntry<float> MenuKeyPressDelay { get; internal set; }
        public static ConfigEntry<float> MenuPostSelectDelay { get; internal set; }


        public static void Settings()
        {

            suitsTerminal.Log.LogInfo("Reading configuration settings");

            //General Configs
            AdvancedTerminalMenu = MakeBool("General", "AdvancedTerminalMenu", true, "Enable this to utilize the advanced menu system and keybinds below");
            ExtensiveLogging = MakeBool("General", "ExtensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            EnforcePaidSuits = MakeBool("General", "EnforcePaidSuits", true, "Enable or Disable enforcing paid suits being locked until they are paid for & unlocked.");
            KeepSuitsWithNegativeIDs = MakeBool("General", "KeepSuitsWithNegativeIDs", false, "Enable this to attempt to keep suits with negative suitIDs, this option could break the mod or others");
            RandomSuitCommand = MakeBool("General", "RandomSuitCommand", false, "Enable/Disable the randomsuit terminal command.");
            ChatCommands = MakeBool("General", "ChatCommands", false, "Enable/Disable suits commands via chat (!suits/!wear).");
            TerminalCommands = MakeBool("General", "TerminalCommands", true, "Enable/Disable the base suits commands via terminal (suits, wear).");
            DontRemove = MakeBool("General", "DontRemove", false, "Enable this to stop this mod from removing suits from the rack and make it compatible with other mods like TooManySuits.");
            DefaultSuit = MakeString("General", "DefaultSuit", "", "Automatically equip this suit when first loading in (in-place of the default orange\nThis configuration option is disabled if SuitSaver is present.");
            DontAddToTerminal = MakeString("General", "DontAddToTerminal", "", "Comma-separated list of suits you do NOT want added to the terminal in any situation. Leave blank to disable this list.");


            //rack config
            SuitsOnRack = MakeClampedInt("Rack Settings", "SuitsOnRack", 13, "Number of suits to keep on the rack. (Up to 13)", 0, 13);
            SuitsOnRackOnly = MakeString("Rack Settings", "SuitsOnRackOnly", "", "Comma-separated list of suits to display on the rack by name. Leave blank to disable this list.\nNOTE: This will make it so ONLY suits listed here will be added to the rack.\nIf no suits match this configuration item, no suits will be added to the rack.");
            DontAddToRack = MakeString("Rack Settings", "DontAddToRack", "", "Comma-separated list of suits you do NOT want added to the rack in any situation. Leave blank to disable this list.");
            HideBoots = MakeBool("Rack Settings", "HideBoots", false, "Enable this to hide the boots by the rack.");
            HideRack = MakeBool("Rack Settings", "HideRack", false, "Enable this to hide the rack, (rack will not be hidden if DontRemove is enabled, SuitsOnRack integer will be ignored if rack hidden).");
            SuitsSortingStyle = MakeClampedString("Rack Settings", "SuitsSortingStyle", "alphabetical (UnlockableName)", "How suits will be sorted in menus & on the rack", new AcceptableValueList<string>("alphabetical", "numerical", "none"));

            //Menu Binds
            FavoritesMenuList = MakeString("AdvancedTerminalMenu", "FavoritesMenuList", "", "Favorited suit names will be stored here and displayed in the AdvancedTerminalMenu.");
            EnablePiPCamera = MakeBool("AdvancedTerminalMenu", "EnablePiPCamera", true, "Disable this to stop the PiP camera from being created");
            MenuLeft = MakeString("AdvancedTerminalMenu", "MenuLeft", "LeftArrow", "Set key to press to go to previous page in advanced menu system");
            MenuRight = MakeString("AdvancedTerminalMenu", "MenuRight", "RightArrow", "Set key to press to go to next page in advanced menu system");
            MenuUp = MakeString("AdvancedTerminalMenu", "MenuUp", "UpArrow", "Set key to press to go to previous item on page in advanced menu system");
            MenuDown = MakeString("AdvancedTerminalMenu", "MenuDown", "DownArrow", "Set key to press to go to next item on page in advanced menu system");
            LeaveMenu = MakeString("AdvancedTerminalMenu", "LeaveMenu", "Backspace", "Set key to press to leave advanced menu system");
            SelectMenu = MakeString("AdvancedTerminalMenu", "SelectMenu", "Enter", "Set key to press to select an item in advanced menu system");
            HelpMenu = MakeString("AdvancedTerminalMenu", "HelpMenu", "H", "Set key to press to toggle help & controls page in advanced menu system");
            FavItemKey = MakeString("AdvancedTerminalMenu", "FavItemKey", "F", "Set key to press to set an item as a favorite in advanced menu system");
            FavMenuKey = MakeString("AdvancedTerminalMenu", "FavMenuKey", "F1", "Set key to press to show favorites menu in advanced menu system");
            TogglePiP = MakeString("AdvancedTerminalMenu", "TogglePiP", "F12", "Set key to press to toggle PiP (mirror cam) on/off in advanced menu system");
            TogglePiPZoom = MakeString("AdvancedTerminalMenu", "TogglePiPZoom", "Minus", "Set key to press to toggle PiP (mirror cam) zoom in advanced menu system");
            TogglePiPRotation = MakeString("AdvancedTerminalMenu", "TogglePiPRotation", "Equals", "Set key to press to toggle PiP (mirror cam) rotation in advanced menu system");
            TogglePiPHeight = MakeString("AdvancedTerminalMenu", "TogglePiPHeight", "Backslash", "Set key to press to toggle PiP (mirror cam) height in advanced menu system");
            
            //Hints
            ChatHints = MakeBool("Hints", "ChatHints", false, "Determines whether chat hints are displayed at load in.");
            BannerHints = MakeBool("Hints", "BannerHints", true, "Determines whether banner hints are displayed at load in.");

            //OpenBodyCams
            UseOpenBodyCams = MakeBool("OpenBodyCams", "UseOpenBodyCams", true, "Disable this to remove the banner hints displayed at load in.");
            ObcResolution = MakeString("OpenBodyCams", "ObcResolution", "1000; 700", "Set the resolution of the Mirror Camera created with OpenBodyCams");

            //Clamped Floats
            MenuKeyPressDelay = MakeClampedFloat("AdvancedTerminalMenu", "MenuKeyPressDelay", 0.15f, "Regular delay when checking for key presses in the AdvancedTerminalMenu. (This delay will be added ontop of MenuPostSelectDelay)", 0.05f, 1f);
            MenuPostSelectDelay = MakeClampedFloat("AdvancedTerminalMenu", "MenuPostSelectDelay", 0.1f, "Delay used after a key press is registered in the AdvancedTerminalMenu.", 0.05f, 1f);

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
