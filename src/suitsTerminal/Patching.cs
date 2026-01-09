using HarmonyLib;
using UnityEngine.InputSystem;
using static suitsTerminal.RackManager;

namespace suitsTerminal;
internal class Patching
{
    //method injected for this patch
    [HarmonyPatch(typeof(UnlockableSuit), "Start")]
    public class SuitStart
    {
        static void Postfix(UnlockableSuit __instance)
        {
            __instance.GetSuitAttributes();
            SuitSpawn(__instance);
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.PositionSuitsOnRack))]
    public class VoidSuitsPositioning
    {
        static bool Prefix()
        {
            if (ModConfig.RackSettings.Value == ModConfig.Removal.DontRemoveAnything)
            {
                Loggers.WARNING("suitsTerminal is NOT touching the rack!!!");
                return true;
            }

            //reset rack suits number
            var rackSuits = AllSuits.FindAll(x => x.IsOnRack);
            foreach (var suit in rackSuits)
                suit.IsOnRack = false;

            foreach (var suit in AllSuits)
            {
                suit.Suit.ResetPosition();
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.SubmitChat_performed))]
    public class Chat_Patch
    {
        static void Prefix(HUDManager __instance, ref InputAction.CallbackContext context)
        {
            if (!ModConfig.ChatCommands.Value)
                return;

            //Loggers.LogDebug("submit chat performed");

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
                            suit.WearSuit();
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
            else if (command.StartsWith("!random")) //random suit command
            {
                int random = Plugin.Rand.Next(AllSuits.Count);
                SuitAttributes suit = AllSuits[random];
                suit.WearSuit();
                HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t Rolled random number [ {random} ]");
                HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t Changing suit to {suit.Name}!");
            }
        }
    }
}
