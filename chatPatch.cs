using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static TerminalApi.TerminalApi;

namespace suitsTerminal
{
    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    public class Chat_Patch : MonoBehaviour
    {
        static void Postfix(ref HUDManager __instance)
        {
            if(SConfig.chatCommands.Value)
            {
                suitsTerminal.X("chat submitted");
                suitsTerminal.X($"lastChatMessage: {__instance.lastChatMessage}");
                string command = __instance.lastChatMessage;
                Terminal getTerm = FindObjectOfType<Terminal>();

                //Set fov with chat command.
                if (command.StartsWith("!suits"))
                {
                    //Terminal getTerm = FindObjectOfType<Terminal>();
                    string displayText = string.Empty;
                    string[] args = command.Split(' ');
                    if (args.Length == 1)
                    {
                        for (int i = 0; i < Suits_Patch.suitsPages.Count; i++)
                        {
                            if (Suits_Patch.suitsPages[i].PageNumber == 1)
                            {
                                displayText = Suits_Patch.suitsPages[i].Content.ToString();
                                displayText = displayText.Replace("> ", "!").Replace("\n", " \t\t").Replace("=", "").Replace(" Choose your Suit! Page", "Page:").Replace("command '", "!").Replace("' to", " to");
                                HUDManager.Instance.AddChatMessage($"[suitsTerminal]:\t{displayText}");
                                break;
                            }
                        }
                        if (displayText == string.Empty)
                        {
                            HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Suits List failed to load.");
                            suitsTerminal.X($"{Suits_Patch.suitsPages.Count}");
                        }
                        return;
                    }
                    else if (args.Length > 1)
                    {
                        string pageNum = args[1];
                        int pageNumVal = Int32.Parse(pageNum);

                        for (int i = 0; i < Suits_Patch.suitsPages.Count; i++)
                        {
                            if (Suits_Patch.suitsPages[i].PageNumber == pageNumVal && Suits_Patch.suitsPages.Count >= pageNumVal)
                            {
                                displayText = Suits_Patch.suitsPages[i].Content.ToString();
                                displayText = displayText.Replace("> ", "!").Replace("\n", " \t\t").Replace("=", "").Replace(" Choose your Suit! Page", "Page:").Replace("command '", "!").Replace("' to", " to");
                                HUDManager.Instance.AddChatMessage($"{displayText}");
                                break;
                            }
                                
                        }
                        if (displayText == string.Empty)
                        {
                            HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Suits Page [{pageNum}] failed to load.");
                            suitsTerminal.X($"string was empty");
                        }
                        return;
                    }

                }
                else if (command.StartsWith("!wear"))
                {
                    string suitName = command.Replace("!wear ", "");
                    string SuitName = string.Empty;
                    int playerID = termPatch.GetPlayerID();
                    suitsTerminal.X($"wear command");

                    if (Suits_Patch.Unlockables != null)
                    {
                        suitsTerminal.X($"Found: {suitName}");
                        foreach (UnlockableSuit suit in Suits_Patch.allSuits)
                        {
                            if (suit.syncedSuitID.Value >= 0)
                            {
                                SuitName = Suits_Patch.Unlockables[suit.syncedSuitID.Value].unlockableName.ToLower();
                                SuitName = termPatch.terminalFriendlyString(SuitName);
                                if (suitName == SuitName)
                                {
                                    UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], suit.syncedSuitID.Value, true);
                                    suit.SwitchSuitServerRpc(playerID);
                                    suit.SwitchSuitClientRpc(playerID);
                                    HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Changing suit to {Suits_Patch.Unlockables[suit.syncedSuitID.Value].unlockableName}\r\n");
                                    break;
                                }
                            }
                        }
                        return;
                    }
                }

                /*    else if (command.StartsWith("!!!cheating") && GameNetworkManager.Instance.isHostingGame)
                    {
                        getTerm.SyncGroupCreditsClientRpc(999999, getTerm.numberOfItemsInDropship);
                        HUDManager.Instance.AddChatMessage("bro wtf stop cheating");
                    } */ //keep disabled outside dev testing :)
            }
        }
    }
}
