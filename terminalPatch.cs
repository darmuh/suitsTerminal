using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Component = UnityEngine.Component;
using System.Text;
using static TerminalApi.TerminalApi;
using GameNetcodeStuff;
using System.Threading.Tasks;
using System.Threading;
using Unity.Collections;
using System.ComponentModel;
using System.Xml;

namespace suitsTerminal
{
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    public class Suits_Patch : MonoBehaviour
    {
        public static List<UnlockableSuit> allSuits = new List<UnlockableSuit>();
        public static List<UnlockableItem> Unlockables = new List<UnlockableItem>();
        public static List<TerminalNode> suitPages = new List<TerminalNode>();
        public static List<string> suitNames = new List<string>();
        public static List<Page> suitsPages = new List<Page> ();
        public static bool keywordsCreated = false;
        public static bool rackSituated = false;
        public static int normSuit = 0;
        public static int reorderSuits = 0;
        public static bool hintOnce = false;
        public static bool isHanging = false;

        static void Postfix()
        {
            normSuit = 0;
            hintOnce = false;
            rackSituated = false;
            InitSuitsTerm();
        }

        public static void InitSuitsTerm()
        {
            suitsTerminal.X("Suits patch");
            // Use Resources.FindObjectsOfTypeAll to find all instances of UnlockableSuit
            allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().ToList();

            // Order the list by syncedSuitID.Value
            allSuits = allSuits.OrderBy((UnlockableSuit suit) => suit.suitID).ToList();

            Unlockables = StartOfRound.Instance.unlockablesList.unlockables;

            // Remove items with negative syncedSuitID

            if (!SConfig.keepSuitsWithNegativeIDs.Value)
            {
                allSuits.RemoveAll(suit => suit.syncedSuitID.Value < 0); //simply remove bad suit IDs
                suitsTerminal.X("Removing suits with negative suitID values.");
            }
            else
            {
                suitsTerminal.X("Attempting to keep suits with negative suitID values.");
                // Remove items with negative syncedSuitID and assign new random numbers
                allSuits.RemoveAll(suit =>
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
                });
            }

            //suitsTerminal.X(allSuits.ToString());

            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {Unlockables.Count}");
            int weirdSuitNum = 0;
            reorderSuits = 0;

            foreach (UnlockableSuit item in allSuits)
            {
                AutoParentToShip component = ((Component)item).gameObject.GetComponent<AutoParentToShip>();
                SuitInfo suitInfoComponent = component.gameObject.GetComponent<SuitInfo>();

                if(!SConfig.dontRemove.Value)
                {
                    isHanging = false;

                    if (suitInfoComponent != null)
                    {
                        string suitTag = suitInfoComponent.suitTag;

                        if (suitTag == "hanging")
                        {
                            isHanging = true;
                            suitsTerminal.X($"Hanging suit detected - {suitTag}");
                        }
                        else if (suitTag == "hidden")
                        {
                            isHanging = false;
                            suitsTerminal.X($"Hidden suit detected - {suitTag}");
                        }
                        else
                        {
                            suitInfoComponent.suitTag = "hidden";
                        }
                    }

                    if (normSuit >= SConfig.suitsOnRack.Value && !rackSituated || (rackSituated && !isHanging))
                    {
                        ProcessHiddenSuit(component);
                        normSuit++;
                    }
                    else if (rackSituated && isHanging)
                    {
                        ProcessHangingSuit(component);
                    }
                    else if (normSuit < SConfig.suitsOnRack.Value && !rackSituated)
                    {
                        ProcessVisibleSuit(component, normSuit);
                        normSuit++;
                    }
                    else
                    {
                        ProcessHiddenSuit(component);
                    }

                    if (normSuit == SConfig.suitsOnRack.Value)
                    {
                        rackSituated = true;
                        suitsTerminal.X("Max suits are on the rack now.");
                    }
                }

                if(SConfig.terminalCommands.Value)
                {
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
                        itemNode.name = SuitName;
                        suitPages.Add(itemNode);
                        AddTerminalKeyword(itemKeyword);
                        suitsTerminal.X($"Keyword for {SuitName} added");
                    }
                    else if (item.syncedSuitID.Value >= 0 && keywordsCreated)
                    {
                        SuitName = Unlockables[item.syncedSuitID.Value].unlockableName;
                        SuitName = terminalFriendlyString(SuitName);
                        if (GetKeyword("wear " + SuitName) != null)
                        {
                            TerminalKeyword itemKeyword = CreateTerminalKeyword("wear " + SuitName, true, itemNode);
                            UpdateKeyword(itemKeyword);
                            itemNode.name = SuitName;
                            suitPages.Add(itemNode);
                            //suitsTerminal.X($"Keyword for {SuitName} updated");
                        }
                        else
                        {
                            TerminalKeyword itemKeyword = CreateTerminalKeyword("wear " + SuitName, true, itemNode);
                            AddTerminalKeyword(itemKeyword);
                            itemNode.name = SuitName;
                            suitPages.Add(itemNode);
                            suitsTerminal.X($"Keyword for {SuitName} added, keyword appears to have been removed.");
                        }

                    }
                    else if (item.syncedSuitID.Value < 0)
                    {
                        weirdSuitNum++;
                        suitsTerminal.X($"Skipping suit with invalid ID number: {item.syncedSuitID.Value}");
                    }
                    else
                    {
                        suitsTerminal.X($"leaving this here but it should never happen");
                    }
                }
                


            }
            suitsTerminal.X($"Suits with invalid ID Numbers: {weirdSuitNum}");
            suitsTerminalCommand();
        }

        private static void ProcessHiddenSuit(AutoParentToShip component)
        {
            SuitInfo suitInfoComponent = component.gameObject.GetComponent<SuitInfo>();

            if (suitInfoComponent == null)
            {
                suitInfoComponent = component.gameObject.AddComponent<SuitInfo>();
            }

            //suitsTerminal.X("processhiddensuit method");
            component.GetComponent<SuitInfo>().suitTag = "hidden";
            component.disableObject = true;
        }

        private static void ProcessHangingSuit(AutoParentToShip component)
        {
            //suitsTerminal.X("processhangingsuit");
            component.disableObject = false;
            component.overrideOffset = true;

            float offsetModifier = 0.18f;

            component.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * offsetModifier * (float)reorderSuits;
            component.rotationOffset = new Vector3(0f, 90f, 0f);
            reorderSuits++;
        }

        private static void ProcessVisibleSuit(AutoParentToShip component, int normSuit)
        {
            //suitsTerminal.X("processvisiblesuit");
            SuitInfo suitInfoComponent = component.gameObject.GetComponent<SuitInfo>();

            if (suitInfoComponent == null)
            {
                suitInfoComponent = component.gameObject.AddComponent<SuitInfo>();
            }

            component.GetComponent<SuitInfo>().suitTag = "hanging";
            component.overrideOffset = true;

            float offsetModifier = 0.18f;

            component.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * offsetModifier * (float)normSuit;
            component.rotationOffset = new Vector3(0f, 90f, 0f);
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

            if (stringBuilder.Length > 14)
            {
                int excessLength = stringBuilder.Length - 14;
                stringBuilder.Remove(14, excessLength);
                //suitsTerminal.X($"terminalFriendlystring: {stringBuilder}");
            }


            return stringBuilder.ToString().ToLower();
        }

        public static void suitsTerminalCommand()
        {
            //suitsTerminal.X("suitsTerminalCommand");

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
                //suitsTerminal.X($"Suits with invalid ID Numbers: {weirdSuitNum}");

                int maxLinesPerPage = 6;
                suitsPages = PageSplitter.SplitTextIntoPages(suitsList.ToString(), maxLinesPerPage);
                
                if(SConfig.terminalCommands.Value)
                {
                    foreach (var page in suitsPages)
                    {
                        if (page.PageNumber == 1)
                        {
                            TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                            TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits", true, suitsNode);
                            suitsNode.name = "suits 1";
                            suitPages.Add(suitsNode);
                            AddTerminalKeyword(suitsKeyword);
                            suitsTerminal.X($"Created main suits command'");
                        }
                        else
                        {
                            TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                            TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits {page.PageNumber}", true, suitsNode);
                            AddTerminalKeyword(suitsKeyword);
                            suitsNode.name = $"suits {page.PageNumber}";
                            suitPages.Add(suitsNode);
                            suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
                        }
                    }
                }
                

                keywordsCreated = true;
            }
            else if(keywordsCreated)
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
                //suitsTerminal.X($"Suits with invalid ID Numbers: {weirdSuitNum}");


                int maxLinesPerPage = 6;
                List<Page> pages = PageSplitter.SplitTextIntoPages(suitsList.ToString(), maxLinesPerPage);

                if (SConfig.terminalCommands.Value)
                {
                    foreach (var page in pages)
                    {
                        if (page.PageNumber == 1)
                        {
                            if (GetKeyword("suits") != null)
                            {
                                TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                                TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits", true, suitsNode);
                                UpdateKeyword(suitsKeyword);
                                //suitsTerminal.X($"Updating main suits command");
                            }
                            else
                            {
                                TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                                TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits", true, suitsNode);
                                AddTerminalKeyword(suitsKeyword);
                                suitsTerminal.X($"main suits command was deleted, creating again");
                            }

                        }
                        else
                        {
                            if (GetKeyword($"suits {page.PageNumber}") != null)
                            {
                                suitsTerminal.X("pages have already been creating, updating keyword");
                                TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                                TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits {page.PageNumber}", true, suitsNode);
                                UpdateKeyword(suitsKeyword);
                                //suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
                            }
                            else
                            {
                                suitsTerminal.X("pages appear to have been deleted, creating keyword again");
                                TerminalNode suitsNode = CreateTerminalNode($"{page.Content.ToString()}", true);
                                TerminalKeyword suitsKeyword = CreateTerminalKeyword($"suits {page.PageNumber}", true, suitsNode);
                                AddTerminalKeyword(suitsKeyword);
                                //suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
                            }
                        }
                    }                
                }
            }

            if(SConfig.randomSuitCommand.Value)
            {
                if (GetKeyword($"randomsuit") == null)
                {
                    suitsTerminal.X("Creating keyword for wearing a random suit");
                    TerminalNode randNode = CreateTerminalNode($"something broke", true, "randomSuit");
                    TerminalKeyword randKeyword = CreateTerminalKeyword($"randomsuit", true, randNode);
                    AddTerminalKeyword(randKeyword);
                }
                else
                {
                    suitsTerminal.X("Creating keyword for wearing a random suit");
                    TerminalNode randNode = CreateTerminalNode($"something broke", true, "randomSuit");
                    TerminalKeyword randKeyword = CreateTerminalKeyword($"randomsuit", true, randNode);
                    UpdateKeyword(randKeyword);
                }
            }
            
                
            if(!hintOnce)
            {
                // Delayed tips
                Task.Run(() =>
                {

                    Thread.Sleep(5000);
                    suitsTerminal.X("hint in chat.");
                    if(SConfig.terminalCommands.Value)
                        HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Access more suits by typing 'suits' in the terminal.");

                    if (SConfig.chatCommands.Value)
                        HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Access more suits by typing '!suits' in chat.");

                });

                Task.Run(() =>
                {
                    Thread.Sleep(20000);
                    suitsTerminal.X("hint on hud.");
                    if (SConfig.suitsOnRack.Value > 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && !SConfig.chatCommands.Value)
                    {
                        HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
                    }
                    else if (SConfig.suitsOnRack.Value == 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && !SConfig.chatCommands.Value)
                    {
                        HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
                    }
                    else if (SConfig.suitsOnRack.Value == 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && SConfig.chatCommands.Value)
                    {
                        HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
                    }
                    else if (SConfig.suitsOnRack.Value > 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && SConfig.chatCommands.Value)
                    {
                        HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
                    }


                });
                hintOnce = true;

            }
            
        }

    }

    public class SuitInfo : MonoBehaviour
    {
        public string suitTag;
    }

    [HarmonyPatch(typeof(StartOfRound), "PositionSuitsOnRack")]
    public class SuitBoughtByOthersPatch
    {
        static void Postfix(StartOfRound __instance)
        {
            
            if(Suits_Patch.keywordsCreated && StartOfRound.Instance.hasHostSpawned)
            {
                suitsTerminal.X("suits rack func called, re-initializing suits");
                Suits_Patch.InitSuitsTerm();
            }
                
        }
    }



    [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
    public class termPatch
    {
        public static string terminalFriendlyString(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            if (stringBuilder.Length > 14)
            {
                int excessLength = stringBuilder.Length - 14;
                stringBuilder.Remove(14, excessLength);
                //suitsTerminal.X($"terminalFriendlystring: {stringBuilder}");
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
                            else
                                suitsTerminal.X($"SuitName: {SuitName} doesn't match Cleaned Text: {cleanedText}");
                        }
                        else
                        {
                            suitsTerminal.X($"suit ID was {suit.syncedSuitID.Value}");
                        }
                    }
                }
                
                __result.displayText = $"Unable to set suit to match command: {cleanedText}";
                
            }

            if (__result.terminalEvent != null && __result.terminalEvent == "randomSuit")
            {
                suitsTerminal.X($"Suit Count: {Suits_Patch.allSuits.Count}");
                suitsTerminal.X($"Unlockables Count: {Suits_Patch.Unlockables.Count}");

                int playerID = GetPlayerID();
                

                if (Suits_Patch.Unlockables != null)
                {
                    for (int i = 0; i < Suits_Patch.Unlockables.Count; i++)
                    {
                        // Get a random index
                        int randomIndex = UnityEngine.Random.Range(0, Suits_Patch.allSuits.Count);
                        string SuitName;

                        // Get the UnlockableSuit at the random index
                        UnlockableSuit randomSuit = Suits_Patch.allSuits[randomIndex];
                        if (randomSuit != null && Suits_Patch.Unlockables[randomSuit.syncedSuitID.Value] != null)
                        {
                            SuitName = Suits_Patch.Unlockables[randomSuit.syncedSuitID.Value].unlockableName;
                            UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], randomSuit.syncedSuitID.Value, true);
                            randomSuit.SwitchSuitServerRpc(playerID);
                            randomSuit.SwitchSuitClientRpc(playerID);
                            __result.displayText = $"Changing suit to {SuitName}!\r\n";
                            break;
                        }
                        else
                        {
                            suitsTerminal.X($"Random suit ID was invalid or null");
                        }
                    }
                    return;
                }

                __result.displayText = $"Unable to set suit random suit.";
            }
        }

        public static int GetPlayerID()
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