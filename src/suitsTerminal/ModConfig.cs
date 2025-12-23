using BepInEx.Configuration;
using static OpenLib.Loggers;
using static OpenLib.ConfigManager.ConfigSetup;

namespace suitsTerminal;
internal class ModConfig
{
    public enum Removal
    {
        DontRemoveAnything,
        OnlyExtraSuits,
        OnlyBootsAndExtraSuits,
        OnlyRackAndExtraSuits,
        Everything
    }

    public enum Hint
    {
        None,
        BannerOnly,
        ChatOnly,
        AllHints
    }

    public enum PiP
    {
        Disabled,
        OpenLib,
        OpenBodyCams
    }

    public enum Page
    {
        Main,
        FavoritesListing,
        SuitsListing,
        HelpPage
    }

    public enum Sort
    {
        None,
        Alphabetical,
        Numerical
    }

    public static ConfigEntry<LoggingLevel> LogLevel { get; internal set; } = null!;
    public static ConfigEntry<bool> RandomSuitCommand { get; internal set; } = null!;
    public static ConfigEntry<bool> ChatCommands { get; internal set; } = null!;
    public static ConfigEntry<int> SuitsOnRack { get; internal set; } = null!;
    public static ConfigEntry<float> RackOffset { get; internal set; } = null!;
    public static ConfigEntry<Removal> RackSettings { get; internal set; } = null!;
    public static ConfigEntry<bool> EnforcePaidSuits { get; internal set; } = null!;
    public static ConfigEntry<Hint> HintStyle { get; internal set; } = null!;
    public static ConfigEntry<PiP> CamStyle { get; internal set; } = null!;
    public static ConfigEntry<string> ObcResolution { get; internal set; } = null!;
            
    public static ConfigEntry<string> MenuUp { get; internal set; } = null!;
    public static ConfigEntry<string> MenuDown { get; internal set; } = null!;
    public static ConfigEntry<string> MenuLeft { get; internal set; } = null!;
    public static ConfigEntry<string> MenuRight { get; internal set; } = null!;
    public static ConfigEntry<string> LeaveMenu { get; internal set; } = null!;
    public static ConfigEntry<string> SelectMenu { get; internal set; } = null!;
    public static ConfigEntry<string> TogglePiP { get; internal set; } = null!;
    public static ConfigEntry<string> TogglePiPZoom { get; internal set; } = null!;
    public static ConfigEntry<string> TogglePiPRotation { get; internal set; } = null!;
    public static ConfigEntry<string> TogglePiPHeight { get; internal set; } = null!;
    public static ConfigEntry<Page> MenuStartPage { get; internal set; } = null!;
            
    public static ConfigEntry<string> SuitsOnRackOnly { get; internal set; } = null!;
    public static ConfigEntry<string> DontAddToRack { get; internal set; } = null!;
    public static ConfigEntry<string> DontAddToTerminal { get; internal set; } = null!;
    public static ConfigEntry<string> FavoritesMenuList { get; internal set; } = null!;
    public static ConfigEntry<string> DefaultSuit { get; internal set; } = null!;
    public static ConfigEntry<bool> PersonalizedFavorites { get; internal set; } = null!;
    public static ConfigEntry<bool> PersonalizedDefault {  get; internal set; } = null!;
    public static ConfigEntry<Sort> SuitsSortingStyle { get; internal set; } = null!;

    public static void Init(ConfigFile config)
    {
        LogLevel = MakeGeneric(config, "Debug", "Logging Level", LoggingLevel.Info, "Set the mod's logging level to determine what messages populate the logs");
        RandomSuitCommand = MakeGeneric(config, "General", "Random Suit Command", false, "Create a random suit command to pick a random suit from the terminal");
        ChatCommands = MakeGeneric(config, "General", "Chat Commands", false, "Create suit related chat commands (!suits/!wear) that can be run from chat");
        EnforcePaidSuits = MakeGeneric(config, "General", "Enforce Paid Suits", true, "Determines whether paid suits need to be unlocked before appearing in the listing.");
        HintStyle = MakeGeneric(config, "General", "Hint Style", Hint.AllHints, "Determines what style of Hints display on player load-in");
        PersonalizedDefault = MakeGeneric(config, "General", "Personalized Default Suit", true, "Your default suit is saved and loaded from a local file. \nUse this if you dont want your default suit to be shared with other players in modpacks/profile codes.");
        DefaultSuit = MakeGeneric(config, "General", "Profile Default Suit", "", "Automatically equip this suit when first loading in (in-place of the default orange suit)\nThis configuration item will not be used if SuitSaver is present or personalized defaults are enabled");
        RackConfigs(config);
        MenuConfigs(config);
        InitControls(config);
    }

    private static void RackConfigs(ConfigFile config)
    {
        SuitsOnRack = MakeGeneric(config, "Rack Settings", "SuitsOnRack", 13, "Number of suits to keep on the rack", 0, 99);
        RackOffset = MakeGeneric(config, "Rack Settings", "Rack Offset", 0.18f, "This determines the gap between each suit on the rack", 0f, 3f);
        RackSettings = MakeGeneric(config, "Rack Settings", "Rack Removal", Removal.OnlyExtraSuits, "This setting determines what of the suits rack is removed, if anything.");
        SuitsOnRackOnly = MakeGeneric(config, "Rack Settings", "Rack Suits (ONLY)", "", "Comma-separated list of suits to display on the rack by name. \nSuits that do not match the names in this list will not be added to the rack. \nLeave blank to allow any suit to be added to the rack.\nNOTE: If no suits match this configuration item when it's populated, no suits will be added to the rack.");
        DontAddToRack = MakeGeneric(config, "Rack Settings", "DONT Add To Rack", "", "Comma-separated list of suits you do NOT want added to the rack in any situation. \nLeave blank to allow any suit to be added to the rack.");
        DontAddToTerminal = MakeGeneric(config, "Rack Settings", "DONT Add To Terminal", "", "Comma-separated list of suits you do NOT want added to the terminal in any situation. \nLeave blank to allow any suit to be added to the terminal.");
        SuitsSortingStyle = MakeGeneric(config, "Menu", "Suit Sorting Style", Sort.Alphabetical, "Determines how the suits listing will be sorted, set to none to ignore any potential sorting");
    }

    private static void MenuConfigs(ConfigFile config)
    {
        MenuStartPage = MakeGeneric(config, "Menu", "Start Page", Page.Main, "Determines what page in the menu you start at when entering the suits command");
        PersonalizedFavorites = MakeGeneric(config, "Menu", "Personalized Favorites", true, "Favorites are saved and loaded from a local file. \nUse this if you dont want your favorites list to be shared with other players in modpacks/profile codes.");
        FavoritesMenuList = MakeGeneric(config, "Menu", "Profile Favorites List", "", "If PersonalizedFavorites is disabled, favorited suit names will be stored and loaded into the menu from here\n");
        CamStyle = MakeGeneric(config, "Menu", "Picture-In-Picture Style", PiP.OpenLib, "Determines what kind of camera is created, if at all");
        ObcResolution = MakeGeneric(config, "Menu", "OpenBodyCams Resolution", "1000; 700", "Set the resolution of the Menu Camera (if created with OpenBodyCams)");
    }

    private static void InitControls(ConfigFile config)
    {
        MenuUp = MakeGeneric(config, "Menu", "Up Control", "UpArrow", "Set key to press to go to previous item on the page");
        MenuDown = MakeGeneric(config, "Menu", "Down Control", "DownArrow", "Set key to press to go to next item on the page");
        MenuLeft = MakeGeneric(config, "Menu", "Left Control", "LeftArrow", "Set key to press to go to previous page");
        MenuRight = MakeGeneric(config, "Menu", "Right Control", "RightArrow", "Set key to press to go to next page");
        LeaveMenu = MakeGeneric(config, "Menu", "Exit Control", "Backspace", "Set key to press to leave menu or go back to parent menu page");
        SelectMenu = MakeGeneric(config, "Menu", "Select Control", "Enter", "Set key to press to select the highlighted menu item on the current page");
        TogglePiP = MakeGeneric(config, "Menu", "Camera Toggle", "F12", "Set key to press to toggle the menu mirror camera on/off");
        TogglePiPZoom = MakeGeneric(config, "Menu", "Camera Zoom", "Minus", "Set key to press to change the zoom level of the menu mirror camera");
        TogglePiPRotation = MakeGeneric(config, "Menu", "Camera Rotation", "Equals", "Set key to press to change the rotation step of the menu mirror camera");
        TogglePiPHeight = MakeGeneric(config, "Menu", "Camera Height", "Backslash", "Set key to press to change the height step of the menu mirror camera");
    }
}
