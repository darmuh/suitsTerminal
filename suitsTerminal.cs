using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace suitsTerminal
{
    [BepInPlugin("darmuh.suitsTerminal", "suitsTerminal", "1.0.0")]

    public class suitsTerminal : BaseUnityPlugin
    {
        public static suitsTerminal instance;
        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.suitsTerminal";
            public const string PLUGIN_NAME = "suitsTerminal";
            public const string PLUGIN_VERSION = "1.0.0";
        }

        internal static new ManualLogSource Log;

        private void Awake()
        {
            suitsTerminal.instance = this;
            suitsTerminal.Log = base.Logger;

            suitsTerminal.X("Plugin suitsTerminal is loaded!");

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
            suitsTerminal.Log.LogInfo(message);
        }

    }
}
