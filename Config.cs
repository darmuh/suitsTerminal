using BepInEx.Configuration;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<int> suitsOnRack;
        public static ConfigEntry<bool> keepSuitsWithNegativeIDs;
        public static ConfigEntry<bool> randomSuitCommand;
        public static ConfigEntry<bool> chatCommands;
        public static ConfigEntry<bool> dontRemove;
        public static ConfigEntry<bool> terminalCommands;


        public static void Settings()
        {

            suitsTerminal.X("Reading configuration settings");

            //Network Configs
            SConfig.suitsOnRack = suitsTerminal.instance.Config.Bind("General", "suitsOnRack", 0, new ConfigDescription("Number of suits to keep on the rack. (Up to 13)", new AcceptableValueRange<int>(0, 13)));
            SConfig.keepSuitsWithNegativeIDs = suitsTerminal.instance.Config.Bind<bool>("General", "keepSuitsWithNegativeIDs", false, "Enable this to attempt to keep suits with negative suitIDs, this option could break the mod or others");
            SConfig.randomSuitCommand = suitsTerminal.instance.Config.Bind<bool>("General", "randomSuitCommand", true, "Enable/Disable the randomsuit terminal command.");
            SConfig.chatCommands = suitsTerminal.instance.Config.Bind<bool>("General", "chatCommands", true, "Enable/Disable suits commands via chat (!suits/!wear).");
            SConfig.dontRemove = suitsTerminal.instance.Config.Bind<bool>("General", "dontRemove", false, "Enable this to stop this mod from removing suits from the rack.");
            SConfig.terminalCommands = suitsTerminal.instance.Config.Bind<bool>("General", "terminalCommands", true, "Enable/Disable the base suits commands via terminal (suits, wear).");

        }
    }
}
