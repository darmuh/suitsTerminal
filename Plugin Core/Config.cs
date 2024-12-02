using BepInEx.Configuration;
using static OpenLib.ConfigManager.ConfigSetup;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> ExtensiveLogging { get; internal set; }
        public static ConfigEntry<int> SuitsOnRack { get; internal set; }
        public static ConfigEntry<bool> HideRack { get; internal set; }
        public static ConfigEntry<bool> HideBoots { get; internal set; }
        public static ConfigEntry<bool> RandomSuitCommand { get; internal set; }
        public static ConfigEntry<bool> ChatCommands { get; internal set; }
        public static ConfigEntry<bool> DontRemove { get; internal set; }
        public static ConfigEntry<bool> EnforcePaidSuits { get; internal set; }
        public static ConfigEntry<bool> TerminalCommands { get; internal set; }
        public static ConfigEntry<bool> AdvancedTerminalMenu { get; internal set; }
        public static ConfigEntry<bool> EnablePiPCamera { get; internal set; }
        public static ConfigEntry<bool> ChatHints { get; internal set; }
        public static ConfigEntry<bool> BannerHints { get; internal set; }
        public static ConfigEntry<bool> UseOpenBodyCams { get; internal set; }
        public static ConfigEntry<string> ObcResolution { get; internal set; }
        public static ConfigEntry<string> DefaultSuit { get; internal set; }


        public static ConfigEntry<string> MenuUp { get; internal set; }
        public static ConfigEntry<string> MenuDown { get; internal set; }
        public static ConfigEntry<string> MenuLeft { get; internal set; }
        public static ConfigEntry<string> MenuRight { get; internal set; }
        public static ConfigEntry<string> LeaveMenu { get; internal set; }
        public static ConfigEntry<string> SelectMenu { get; internal set; }
        public static ConfigEntry<string> HelpMenu { get; internal set; }
        public static ConfigEntry<string> FavItemKey { get; internal set; }
        public static ConfigEntry<string> FavMenuKey { get; internal set; }
        public static ConfigEntry<string> TogglePiP { get; internal set; }
        public static ConfigEntry<string> TogglePiPZoom { get; internal set; }
        public static ConfigEntry<string> TogglePiPRotation { get; internal set; }
        public static ConfigEntry<string> TogglePiPHeight { get; internal set; }

        public static ConfigEntry<string> SuitsOnRackOnly { get; internal set; }
        public static ConfigEntry<string> DontAddToRack { get; internal set; }
        public static ConfigEntry<string> DontAddToTerminal { get; internal set; }
        public static ConfigEntry<string> FavoritesMenuList { get; internal set; }
        public static ConfigEntry<bool> PersonalizedFavorites { get; internal set; }
        public static ConfigEntry<string> SuitsSortingStyle { get; internal set; }

        public static ConfigEntry<float> MenuKeyPressDelay { get; internal set; }
        public static ConfigEntry<float> MenuPostSelectDelay { get; internal set; }


        public static void Settings()
        {

            Plugin.Log.LogInfo("Reading configuration settings");

            //General Configs
            AdvancedTerminalMenu = MakeBool(Plugin.instance.Config, "General", "AdvancedTerminalMenu", true, "Enable this to utilize the advanced menu system and keybinds below");
            ExtensiveLogging = MakeBool(Plugin.instance.Config, "General", "ExtensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            EnforcePaidSuits = MakeBool(Plugin.instance.Config, "General", "EnforcePaidSuits", true, "Enable or Disable enforcing paid suits being locked until they are paid for & unlocked.");
            RandomSuitCommand = MakeBool(Plugin.instance.Config, "General", "RandomSuitCommand", false, "Enable/Disable the randomsuit terminal command.");
            ChatCommands = MakeBool(Plugin.instance.Config, "General", "ChatCommands", false, "Enable/Disable suits commands via chat (!suits/!wear).");
            TerminalCommands = MakeBool(Plugin.instance.Config, "General", "TerminalCommands", true, "Enable/Disable the base suits commands via terminal (suits, wear).");
            DontRemove = MakeBool(Plugin.instance.Config, "General", "DontRemove", false, "Enable this to stop this mod from removing suits from the rack and make it compatible with other mods like TooManySuits.");
            DefaultSuit = MakeString(Plugin.instance.Config, "General", "DefaultSuit", "", "Automatically equip this suit when first loading in (in-place of the default orange\nThis configuration option is disabled if SuitSaver is present.");
            DontAddToTerminal = MakeString(Plugin.instance.Config, "General", "DontAddToTerminal", "", "Comma-separated list of suits you do NOT want added to the terminal in any situation. Leave blank to disable this list.");

            //rack config
            SuitsOnRack = MakeClampedInt(Plugin.instance.Config, "Rack Settings", "SuitsOnRack", 13, "Number of suits to keep on the rack. (Up to 13)", 0, 13);
            SuitsOnRackOnly = MakeString(Plugin.instance.Config, "Rack Settings", "SuitsOnRackOnly", "", "Comma-separated list of suits to display on the rack by name. Leave blank to disable this list.\nNOTE: This will make it so ONLY suits listed here will be added to the rack.\nIf no suits match this configuration item, no suits will be added to the rack.");
            DontAddToRack = MakeString(Plugin.instance.Config, "Rack Settings", "DontAddToRack", "", "Comma-separated list of suits you do NOT want added to the rack in any situation. Leave blank to disable this list.");
            HideBoots = MakeBool(Plugin.instance.Config, "Rack Settings", "HideBoots", false, "Enable this to hide the boots by the rack.");
            HideRack = MakeBool(Plugin.instance.Config, "Rack Settings", "HideRack", false, "Enable this to hide the rack, (rack will not be hidden if DontRemove is enabled, SuitsOnRack integer will be ignored if rack hidden).");
            SuitsSortingStyle = MakeClampedString(Plugin.instance.Config, "Rack Settings", "SuitsSortingStyle", "alphabetical (UnlockableName)", "How suits will be sorted in menus & on the rack", new AcceptableValueList<string>("alphabetical", "numerical", "none"));

            //Menu Binds
            FavoritesMenuList = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "FavoritesMenuList", "", "Favorited suit names will be stored here and displayed in the AdvancedTerminalMenu.");
            PersonalizedFavorites = MakeBool(Plugin.instance.Config, "AdvancedTerminalMenu", "PersonalizedFavorites", false, "Enable this to ignore the FavoritesMenuList configuration item in favor of a personal file saving your favorites list.\nUse this if you dont want your favorites list to be shared with other players in modpacks/profile codes.");
            EnablePiPCamera = MakeBool(Plugin.instance.Config, "AdvancedTerminalMenu", "EnablePiPCamera", true, "Disable this to stop the PiP camera from being created");
            MenuLeft = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "MenuLeft", "LeftArrow", "Set key to press to go to previous page in advanced menu system");
            MenuRight = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "MenuRight", "RightArrow", "Set key to press to go to next page in advanced menu system");
            MenuUp = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "MenuUp", "UpArrow", "Set key to press to go to previous item on page in advanced menu system");
            MenuDown = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "MenuDown", "DownArrow", "Set key to press to go to next item on page in advanced menu system");
            LeaveMenu = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "LeaveMenu", "Backspace", "Set key to press to leave advanced menu system");
            SelectMenu = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "SelectMenu", "Enter", "Set key to press to select an item in advanced menu system");
            HelpMenu = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "HelpMenu", "H", "Set key to press to toggle help & controls page in advanced menu system");
            FavItemKey = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "FavItemKey", "F", "Set key to press to set an item as a favorite in advanced menu system");
            FavMenuKey = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "FavMenuKey", "F1", "Set key to press to show favorites menu in advanced menu system");
            TogglePiP = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "TogglePiP", "F12", "Set key to press to toggle PiP (mirror cam) on/off in advanced menu system");
            TogglePiPZoom = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "TogglePiPZoom", "Minus", "Set key to press to toggle PiP (mirror cam) zoom in advanced menu system");
            TogglePiPRotation = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "TogglePiPRotation", "Equals", "Set key to press to toggle PiP (mirror cam) rotation in advanced menu system");
            TogglePiPHeight = MakeString(Plugin.instance.Config, "AdvancedTerminalMenu", "TogglePiPHeight", "Backslash", "Set key to press to toggle PiP (mirror cam) height in advanced menu system");

            //Hints
            ChatHints = MakeBool(Plugin.instance.Config, "Hints", "ChatHints", false, "Determines whether chat hints are displayed at load in.");
            BannerHints = MakeBool(Plugin.instance.Config, "Hints", "BannerHints", true, "Determines whether banner hints are displayed at load in.");

            //OpenBodyCams
            UseOpenBodyCams = MakeBool(Plugin.instance.Config, "OpenBodyCams", "UseOpenBodyCams", true, "Disable this to remove the banner hints displayed at load in.");
            ObcResolution = MakeString(Plugin.instance.Config, "OpenBodyCams", "ObcResolution", "1000; 700", "Set the resolution of the Mirror Camera created with OpenBodyCams");

            //Clamped Floats
            //MenuKeyPressDelay = MakeClampedFloat("AdvancedTerminalMenu", "MenuKeyPressDelay", 0.15f, "Regular delay when checking for key presses in the AdvancedTerminalMenu. (This delay will be added ontop of MenuPostSelectDelay)", 0.05f, 1f);
            //MenuPostSelectDelay = MakeClampedFloat("AdvancedTerminalMenu", "MenuPostSelectDelay", 0.1f, "Delay used after a key press is registered in the AdvancedTerminalMenu.", 0.05f, 1f);

            RemoveOrphanedEntries(Plugin.instance.Config);
            Plugin.instance.Config.Save(); // Save the config file
            Plugin.instance.Config.SettingChanged += OnSettingChanged;
        }

        private static void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
        {
            if (settingChangedArg.ChangedSetting == null)
                return;

            Plugin.X("CONFIG SETTING CHANGE EVENT");
            
            if(settingChangedArg.ChangedSetting == PersonalizedFavorites)
                AllSuits.InitFavoritesListing(true);

            if(settingChangedArg.ChangedSetting.Definition.Section == "Rack Settings")
            {
                Misc.resetSuitPlacementOnRestart = true;
            }
        }
    }
}
