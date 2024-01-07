using BepInEx.Configuration;

namespace suitsTerminal
{
    public static class SConfig
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<int> suitsOnRack; //lobby name command
        public static ConfigEntry<bool> keepSuitsWithNegativeIDs;


        public static void Settings()
        {

            suitsTerminal.X("Reading configuration settings");

            //Network Configs
            SConfig.suitsOnRack = suitsTerminal.instance.Config.Bind("General", "suitsOnRack", 0, new ConfigDescription("Number of suits to keep on the rack. (Up to 13)", new AcceptableValueRange<int>(0, 13)));
            SConfig.keepSuitsWithNegativeIDs = suitsTerminal.instance.Config.Bind<bool>("General", "keepSuitsWithNegativeIDs", false, "Enable this to attempt to keep suits with negative suitIDs, this option could break the mod or others");
        }
    }
}
