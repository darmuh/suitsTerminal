﻿using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using static suitsTerminal.Misc;

namespace suitsTerminal
{
    [BepInPlugin("darmuh.suitsTerminal", "suitsTerminal", "1.3.0")]

    public class suitsTerminal : BaseUnityPlugin
    {
        public static suitsTerminal instance;
        internal static bool TerminalStuff = false;
        internal static bool OpenBodyCams = false;

        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.suitsTerminal";
            public const string PLUGIN_NAME = "suitsTerminal";
            public const string PLUGIN_VERSION = "1.3.0";
        }

        public static Terminal Terminal;

        internal static new ManualLogSource Log;

        private void Awake()
        {
            instance = this;
            Log = base.Logger;

            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} version {PluginInfo.PLUGIN_VERSION} has been started!");
            keywordsCreated = false;
            SConfig.Settings();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            //LeaveTerminal.AddTest(); //this command is only for devtesting
            //Addkeywords used to be here

            //start of networking stuff
            /*
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            */
            //end of networking stuff
        }

        public static void X(string message)
        {
            if (!SConfig.extensiveLogging.Value)
                return;

            suitsTerminal.Log.LogInfo(message);
        }

    }
}
