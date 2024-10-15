using HarmonyLib;
using UnityEngine;

namespace suitsTerminal
{
    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    public class Chat_Patch : MonoBehaviour
    {
        internal static string lastMessage = "";
        static void Postfix(HUDManager __instance)
        {
            if (SConfig.ChatCommands.Value)
            {
                if (lastMessage == __instance.lastChatMessage) //avoid patching for every single chat message
                    return;

                Plugin.X($"Testing message [{__instance.lastChatMessage}] for command.");

                string command = __instance.lastChatMessage;

                ChatHandler.HandleChatMessage(command);
            }
        }
    }
}
