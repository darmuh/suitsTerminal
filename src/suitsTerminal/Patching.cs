using System.Linq;
using HarmonyLib;
using UnityEngine.InputSystem;
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

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.UnlockShipObject))]
    public class CatchSuitSpawnsOnReload
    {
        static void Postfix(StartOfRound __instance, int unlockableID)
        {
            if (!Plugin.HintOnce) //dont rely on this patch on first launch of a lobby
                return;

            if (__instance.unlockablesList.unlockables.Count <= unlockableID)
                return;

            if (__instance.unlockablesList.unlockables[unlockableID].unlockableType != 0)
                return;

            string name = __instance.unlockablesList.unlockables[unlockableID].unlockableName;

            if (AllSuits.Any(x => x.Name == name))
                Loggers.LogInfo($"SuitAttributes for [{name}] already exists!");
            else
            {
                Loggers.LogInfo("New suit detected! Calling upon suitsTerminal rack manager to ensure it is placed by user's config!");
                RackLaunch();
            }
        }
    }

    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.SubmitChat_performed))]
    public class Chat_Patch
    {
        static void Postfix(HUDManager __instance, ref InputAction.CallbackContext context)
        {
            if (!ModConfig.ChatCommands.Value)
                return;

            var localPlayer = Plugin.LocalPlayer;
            if (!context.performed || localPlayer == null || !localPlayer.isTypingChat || localPlayer.isPlayerDead)
                return;

            string message = __instance.chatTextField.text.Trim();
            Loggers.LogMessage($"Testing message [{message}] for command.");
            HandleChatMessage(message);
        }

        private static string[] GetArgs(string command)
        {
            return command.Split(' ');
        }

        internal static void HandleChatMessage(string command)
        {
            if (command.StartsWith("!suits")) //display suits listing
            {
                string[] args = GetArgs(command);
                if (args.Length == 1) //display first page
                {
                    string message = Plugin.ChatListing(6, 1);
                    HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t {message}");
                    return;
                }
                else if (args.Length > 1) //listing has page number argument
                {
                    string pageNum = args[1];
                    if (int.TryParse(pageNum, out int pageNumVal))
                    {
                        string message = Plugin.ChatListing(6, pageNumVal);
                        HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t {message}");
                        return;
                    }
                    else
                    {
                        HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t Invalid page number format: {pageNum}");
                        Loggers.WARNING($"Invalid page number format: {pageNum}");
                        return;
                    }
                }
            }
            else if (command.StartsWith("!wear")) //wear command
            {
                string[] args = GetArgs(command);
                if (args.Length <= 1) //need arg with suit number
                    HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t No suit specified...");
                else
                {
                    string suitNum = args[1];
                    if (int.TryParse(suitNum, out int suitNumVal)) //able to get number from arg
                    {
                        if (suitNumVal >= 0 && suitNumVal < AllSuits.Count) //valid suit number
                        {
                            SuitAttributes suit = AllSuits[suitNumVal];
                            Loggers.LogDebug($"wear command");
                            BetterSuitPick(suit);
                            return;
                        }
                        else
                        {
                            HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t Invalid suit number: {suitNum}");
                            Loggers.WARNING($"Invalid suit number: {suitNum}");
                            return;
                        }
                    }
                    else
                    {
                        HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t Invalid suit number format: {suitNum}");
                        Loggers.WARNING($"Invalid suit number format: {suitNum}");
                        return;
                    }
                }
            }
            else if (command.StartsWith("!clear")) //clear chat history
            {
                _ = HUDManager.Instance.chatText.text[HUDManager.Instance.chatText.text.Length..];
                HUDManager.Instance.ChatMessageHistory.Clear();
            }
        }
    }
}
