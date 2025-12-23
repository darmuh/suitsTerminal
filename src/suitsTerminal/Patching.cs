using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using static suitsTerminal.CommandHandler;
using static suitsTerminal.RackManager;

namespace suitsTerminal;
internal class Patching
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.PositionSuitsOnRack))]
    public class PositionSuitsOnRackPatch
    {
        static void Postfix()
        {
            if (!RackSetupComplete)
                return;

            RackLaunch();
        }
    }

    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.SubmitChat_performed))]
    public class Chat_Patch
    {
        internal static string lastMessage = "";
        internal static string lastCommandRun = "";
        static void Postfix(HUDManager __instance)
        {
            if (!ModConfig.ChatCommands.Value)
                return;

            if (lastMessage == __instance.lastChatMessage) //avoid patching for every single chat message
                return;

            Loggers.LogInfo($"Testing message [{__instance.lastChatMessage}] for command.");

            string command = __instance.lastChatMessage;

            HandleChatMessage(command);
        }

        private static string[] GetArgs(string command)
        {
            return command.Split(' ');
        }

        internal static void HandleChatMessage(string command)
        {
            if (lastCommandRun == command)
                return;

            lastCommandRun = command;

            //Set fov with chat command.
            if (command.StartsWith("!suits"))
            {
                string[] args = GetArgs(command);
                if (args.Length == 1)
                {
                    string message = Plugin.ChatListing(6, 1);
                    HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                    return;
                }
                else if (args.Length > 1)
                {
                    string pageNum = args[1];
                    if (int.TryParse(pageNum, out int pageNumVal))
                    {
                        string message = Plugin.ChatListing(6, pageNumVal);
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                        return;
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid page number format: {pageNum}");
                        Loggers.WARNING($"Invalid page number format: {pageNum}");
                        return;
                    }
                }
            }
            else if (command.StartsWith("!wear"))
            {
                string[] args = GetArgs(command);
                if (args.Length <= 1)
                    HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t No suit specified...");
                else
                {
                    string suitNum = args[1];
                    if (int.TryParse(suitNum, out int suitNumVal))
                    {
                        if (suitNumVal >= 0 && suitNumVal < RackManager.AllSuits.Count)
                        {
                            SuitAttributes suit = RackManager.AllSuits[suitNumVal];
                            Loggers.LogDebug($"wear command");
                            BetterSuitPick(suit);
                            return;
                        }
                        else
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number: {suitNum}");
                            Loggers.WARNING($"Invalid suit number: {suitNum}");
                            return;
                        }
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number format: {suitNum}");
                        Loggers.WARNING($"Invalid suit number format: {suitNum}");
                        return;
                    }
                }
            }
            else if (command.StartsWith("!clear"))
            {
                _ = HUDManager.Instance.chatText.text[HUDManager.Instance.chatText.text.Length..];
                HUDManager.Instance.ChatMessageHistory.Clear();
            }
        }
    }
}
