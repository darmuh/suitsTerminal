using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Steamworks;
using System.Text;
using static TerminalApi.TerminalApi;
using GameNetcodeStuff;
using Unity.Netcode;
using Steamworks.Ugc;
using System.Threading.Tasks;
using System.Threading;

namespace suitsTerminal
{
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    public class Suits_Patch : MonoBehaviour
    {
        public static List<UnlockableSuit> allSuits = new List<UnlockableSuit>();
        public static List<UnlockableItem> Unlockables = new List<UnlockableItem>();
        public static List<string> suitNames = new List<string>();
        public static bool keywordsCreated = false;

        static void Postfix()
        {
                
            suitsTerminal.X("Suits patch");
            // Use Resources.FindObjectsOfTypeAll to find all instances of UnlockableSuit
            allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().ToList();

            // Order the list by syncedSuitID.Value
            allSuits = allSuits.OrderBy((UnlockableSuit suit) => suit.suitID).ToList();

            Unlockables = StartOfRound.Instance.unlockablesList.unlockables;

            // Remove items with negative syncedSuitID
            allSuits.RemoveAll(suit => suit.syncedSuitID.Value < 0); //simply remove bad suit IDs

            // Remove items with negative syncedSuitID and assign new random numbers
     /*       allSuits.RemoveAll(suit =>
            {
                if (suit.syncedSuitID.Value < 0)
                {
                    // Generate a new random number
                    int newRandomNumber;
                    do
                    {
                        newRandomNumber = UnityEngine.Random.Range(1, int.MaxValue);
                    } while (allSuits.Any(otherSuit => otherSuit.syncedSuitID.Value == newRandomNumber));

                    // Assign the new random number
                    suitsTerminal.X($"suit ID was {suit.syncedSuitID.Value}");
                    suit.syncedSuitID.Value = newRandomNumber;
                    suitsTerminal.X($"suit ID changed to {suit.syncedSuitID.Value}");

                    return true; // Remove the item
                }

                return false; // Keep the item
            }); */

            //suitsTerminal.X(allSuits.ToString());

            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {Unlockables.Count}");
            int weirdSuitNum = 0;
            foreach (UnlockableSuit item in allSuits)
            {
                
                AutoParentToShip component = ((Component)item).gameObject.GetComponent<AutoParentToShip>();
                component.disableObject = true;
                TerminalNode itemNode = CreateTerminalNode("this shouldn't show", true, "switchSuit");
                string SuitName = "";
                if (item.syncedSuitID.Value >= 0 && !keywordsCreated)
                {
                    SuitName = Unlockables[item.syncedSuitID.Value].unlockableName;
                    SuitName = terminalFriendlyString(SuitName);
                    if (suitNames.Contains(SuitName.ToLower()))
                    {
                        SuitName = SuitName + "z";
                        suitsTerminal.X($"Duplicate found. Updated SuitName: {SuitName}");
                    }
                    suitNames.Add(SuitName.ToLower());
                    TerminalKeyword itemKeyword = CreateTerminalKeyword("wear " + SuitName, true, itemNode);
                    AddTerminalKeyword(itemKeyword);
                    suitsTerminal.X($"Keyword for {SuitName} added");
                }
                else if(item.syncedSuitID.Value >= 0 && keywordsCreated && suitsTerminal.instance.CompatibilityAC)
                {
                    SuitName = Unlockables[item.syncedSuitID.Value].unlockableName;
                    SuitName = terminalFriendlyString(SuitName);
                    TerminalKeyword itemKeyword = CreateTerminalKeyword("wear " + SuitName, true, itemNode);
                    AddTerminalKeyword(itemKeyword);
                    suitsTerminal.X($"Keyword for {SuitName} updated");
                }
                else if (item.syncedSuitID.Value < 0)
                {
                    weirdSuitNum++;
                    suitsTerminal.X($"Skipping suit with invalid ID number: {item.syncedSuitID.Value}");
                }
                else
                {
                    suitsTerminal.X($"Keywords already created");
                    suitsTerminal.X($"Item: {Unlockables[item.syncedSuitID.Value].unlockableName}");
                }


            }
            suitsTerminal.X($"Suits with invalid ID Numbers: {weirdSuitNum}");
            suitsTerminalCommand();

        }

        private static string terminalFriendlyString(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        public static void suitsTerminalCommand()
        {
            suitsTerminal.X("suitsTerminalCommand");

            Terminal getTerm = Object.FindObjectOfType<Terminal>();

            if (getTerm != null && !keywordsCreated)
            {
                
                StringBuilder suitsList = new StringBuilder($"");
                
                int weirdSuitNum = 0;
                foreach (UnlockableSuit item in allSuits)
                {
                    
                    string SuitName = "";
                    if (item.syncedSuitID.Value >= 0)
                    {
                        SuitName = Unlockables[item.syncedSuitID.Value].unlockableName;
                        SuitName = terminalFriendlyString(SuitName);
                        suitsList.AppendLine($"> wear {SuitName}\n");
                    }
                    else
                    {
                        weirdSuitNum++;
                        suitsTerminal.X($"Skipping suit.");
                    }
                    
                }
                suitsTerminal.X($"Suits with invalid ID Numbers: {weirdSuitNum}");

                int maxLinesPerPage = 6;
                List<Page> pages = PageSplitter.SplitTextIntoPages(suitsList.ToString(), maxLinesPerPage);
                foreach (var page in pages)
                {
                    if (page.PageNumber == 1)
                    {
                        TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                        TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits", true, suitsNode);
                        AddTerminalKeyword(suitsKeyword);
                        suitsTerminal.X($"Created main suits command'");
                    }
                    else
                    {
                        TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                        TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits {page.PageNumber}", true, suitsNode);
                        AddTerminalKeyword(suitsKeyword);
                        suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
                    }
                }

                keywordsCreated = true;
            }
            else if(suitsTerminal.instance.CompatibilityAC)
            {
                //verify keywords have been created

                StringBuilder suitsList = new StringBuilder($"");

                int weirdSuitNum = 0;
                foreach (UnlockableSuit item in allSuits)
                {

                    string SuitName = "";
                    if (item.syncedSuitID.Value >= 0)
                    {
                        SuitName = Unlockables[item.syncedSuitID.Value].unlockableName;
                        SuitName = terminalFriendlyString(SuitName);
                        suitsList.AppendLine($"> wear {SuitName}\n");
                    }
                    else
                    {
                        weirdSuitNum++;
                        suitsTerminal.X($"Skipping suit.");
                    }

                }
                suitsTerminal.X($"Suits with invalid ID Numbers: {weirdSuitNum}");


                int maxLinesPerPage = 6;
                List<Page> pages = PageSplitter.SplitTextIntoPages(suitsList.ToString(), maxLinesPerPage);
                foreach (var page in pages)
                {
                    if (page.PageNumber == 1)
                    {
                        TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                        TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits", true, suitsNode);
                        AddTerminalKeyword(suitsKeyword);
                        suitsTerminal.X($"Created main suits command'");
                    }
                    else
                    {
                        TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                        TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits {page.PageNumber}", true, suitsNode);
                        AddTerminalKeyword(suitsKeyword);
                        suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
                    }
                }
            }

            // Delayed tips
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                suitsTerminal.X("hint in chat.");
                HUDManager.Instance.AddTextToChatOnServer($"Access more suits by typing 'suits' in the terminal.");
            });

            Task.Run(() =>
            {
                Thread.Sleep(20000);
                suitsTerminal.X("hint on hud.");
                HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
                
            });
        }

    }

    [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
    public class termPatch
    {
        private static string terminalFriendlyString(string s) 
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        static void Postfix(Terminal __instance, ref TerminalNode __result)
        {
            
            if (__result.terminalEvent != null && __result.terminalEvent == "switchSuit")
            {

                suitsTerminal.X($"Suit Count: {Suits_Patch.allSuits.Count}");
                suitsTerminal.X($"Unlockables Count: {Suits_Patch.Unlockables.Count}");

                string cleanedText = GetScreenCleanedText(__instance);
                //ulong playerID = StartOfRound.Instance.localPlayerController.playerClientId;
                int playerID = GetPlayerID();
                string SuitName = string.Empty;
                if(Suits_Patch.Unlockables != null)
                {
                    foreach (UnlockableSuit suit in Suits_Patch.allSuits)
                    {
                        if (suit.syncedSuitID.Value >= 0)
                        {
                            SuitName = Suits_Patch.Unlockables[suit.syncedSuitID.Value].unlockableName.ToLower();
                            SuitName = terminalFriendlyString(SuitName);
                            if (cleanedText.Equals("wear " + SuitName))
                            {
                                UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], suit.syncedSuitID.Value, true);
                                suit.SwitchSuitServerRpc(playerID);
                                suit.SwitchSuitClientRpc(playerID);
                                __result.displayText = $"Changing suit to {Suits_Patch.Unlockables[suit.syncedSuitID.Value].unlockableName}\r\n";
                                return;
                            }
                        }
                        else
                        {
                            suitsTerminal.X($"suit ID was {suit.syncedSuitID.Value}");
                        }
                    }
                }

                __result.displayText = $"Unable to set suit to match command: {cleanedText}";
                
            }
        }

        private static int GetPlayerID()
        {
            List<PlayerControllerB> allPlayers = new List<PlayerControllerB>();
            string myName = GameNetworkManager.Instance.localPlayerController.playerUsername;
            int returnID = -1;
            allPlayers = StartOfRound.Instance.allPlayerScripts.ToList();
            allPlayers = allPlayers.OrderBy((PlayerControllerB player) => player.playerClientId).ToList();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if(StartOfRound.Instance.allPlayerScripts[i].playerUsername == myName)
                {
                    suitsTerminal.X("Found my playerID");
                    returnID = i;
                    break;
                }
            }
            if (returnID == -1)
                suitsTerminal.X("Failed to find ID");
            return returnID;
        }

        private static string GetScreenCleanedText(Terminal __instance)
        {
            string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            return RemovePunctuation(s);
        }

        private static string RemovePunctuation(string s) //copied from game files
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }
    }

    public class Page
    {
        public StringBuilder Content { get; set; }
        public int PageNumber { get; set; }
    }

    public class PageSplitter
    {
        public static List<Page> SplitTextIntoPages(string inputText, int maxLinesPerPage)
        {
            string[] lines = inputText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            List<Page> pages = new List<Page>();
            int lineNumber = 0;
            int pageNumber = 1;

            while (lineNumber < lines.Length)
            {
                Page page = new Page { Content = new StringBuilder(), PageNumber = pageNumber };

                // Add header for each page
                page.Content.AppendLine($"=== Choose your Suit! Page {pageNumber} ===\r\n\r\n");

                for (int i = 0; i < maxLinesPerPage && lineNumber < lines.Length; i++)
                {
                    page.Content.AppendLine(lines[lineNumber]);
                    lineNumber++;
                }

                if (lineNumber < lines.Length)
                {
                    page.Content.AppendLine($"> Use command 'suits {pageNumber + 1}' to see the next page of suits!\r\n");
                }

                pages.Add(page);
                pageNumber++;
            }

            return pages;
        }
    }
}