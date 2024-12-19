using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using suitsTerminal.EventSub;
using System.Reflection;
using static suitsTerminal.Misc;

namespace suitsTerminal
{
    [BepInPlugin("darmuh.suitsTerminal", "suitsTerminal", PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("darmuh.OpenLib", "0.2.11")]

    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        internal static bool TerminalStuff = false;
        internal static bool SuitSaver = false;
        internal static bool TooManySuits = false;
        //internal static MainListing sT = new();

        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.suitsTerminal";
            public const string PLUGIN_NAME = "suitsTerminal";
            public const string PLUGIN_VERSION = "1.5.11";
        }

        public static Terminal Terminal;

        internal static new ManualLogSource Log;

        private void Awake()
        {
            instance = this;
            Log = base.Logger;
            //CommandRegistry.InitListing(ref sT); //using openLib defaultListing
            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} version {PluginInfo.PLUGIN_VERSION} has been started!");
            keywordsCreated = false;
            SConfig.Settings();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Subscribers.Subscribe();
        }

        public static void X(string message)
        {
            if (!SConfig.ExtensiveLogging.Value)
                return;

            Log.LogInfo(message);
        }

        public static void WARNING(string message)
        {
            Log.LogWarning(message);
        }

        public static void ERROR(string message)
        {
            Log.LogError(message);
        }

    }
}
