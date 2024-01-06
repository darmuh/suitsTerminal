using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using TerminalApi;
using Unity.Netcode;
using UnityEngine;

namespace suitsTerminal
{
    [BepInPlugin("darmuh.suitsTerminal", "suitsTerminal", "1.0.1")]

    public class suitsTerminal : BaseUnityPlugin
    {
        public static suitsTerminal instance;
        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.suitsTerminal";
            public const string PLUGIN_NAME = "suitsTerminal";
            public const string PLUGIN_VERSION = "1.0.2";
        }

        public bool CompatibilityAC = false;

        internal static new ManualLogSource Log;

        private void Awake()
        {
            suitsTerminal.instance = this;
            suitsTerminal.Log = base.Logger;

            suitsTerminal.X("Plugin suitsTerminal is loaded!");
            Suits_Patch.keywordsCreated = false;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            if (Chainloader.PluginInfos.ContainsKey("com.potatoepet.AdvancedCompany"))
            {
                suitsTerminal.X("Advanced Company detected, setting Advanced Company Compatibility options");
                CompatibilityAC = true;
            }

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
            suitsTerminal.Log.LogInfo(message);
        }

    }
}
