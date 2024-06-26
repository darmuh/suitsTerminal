using HarmonyLib;
using System;
using UnityEngine;
using static suitsTerminal.AllSuits;
using static suitsTerminal.StringStuff;
using static suitsTerminal.CommandHandler;

namespace suitsTerminal
{
    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    public class Chat_Patch : MonoBehaviour
    {
        internal static string lastMessage = "";
        static void Postfix(HUDManager __instance)
        {
            if(SConfig.chatCommands.Value)
            {
                if (lastMessage == __instance.lastChatMessage)
                    return;

                suitsTerminal.X($"Testing message [{__instance.lastChatMessage}] for command.");

                string command = __instance.lastChatMessage;

                ChatHandler.HandleChatMessage(command);
            }
        }
    }
}
