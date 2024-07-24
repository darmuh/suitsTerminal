using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static suitsTerminal.AllSuits;
using static suitsTerminal.StringStuff;
using OpenLib.CoreMethods;
using OpenBodyCams;

namespace suitsTerminal
{
    internal class CommandHandler
    {
        public delegate void CommandDelegate(out string displayText);
        internal static string RandomSuit()
        {
            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {UnlockableItems.Count}");

            string displayText;

            if (UnlockableItems != null)
            {
                for (int i = 0; i < UnlockableItems.Count; i++)
                {
                    // Get a random index
                    int randomIndex = UnityEngine.Random.Range(0, allSuits.Count);
                    string SuitName;

                    // Get the UnlockableSuit at the random index
                    UnlockableSuit randomSuit = allSuits[randomIndex];
                    if (randomSuit != null && UnlockableItems[randomSuit.syncedSuitID.Value] != null)
                    {
                        SuitName = UnlockableItems[randomSuit.syncedSuitID.Value].unlockableName;
                        randomSuit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
                        displayText = $"Changing suit to {SuitName}!\r\n";
                        return displayText;
                    }
                    else
                    {
                        suitsTerminal.X($"Random suit ID was invalid or null");
                    }
                }
            }

            displayText = $"Unable to set suit random suit.\r\n";
            return displayText;
        }

        internal static string SuitPickCommand()
        {
            string displayText = PickSuit();
            return displayText;
        }

        internal static string AdvancedSuitsTerm()
        {
            suitsTerminal.Terminal.StartCoroutine(AdvancedMenu.ActiveMenu());
            string displayText = AdvancedMenuDisplay(suitNames, 0, 10, 1);
            return displayText;
        }

        private static void PickSuitBasedOnID(int suitID, out UnlockableSuit suitToUse)
        {
            foreach (UnlockableSuit suit in allSuits)
            {
                if (suit.syncedSuitID.Value == suitID)
                {
                    suitToUse = suit;
                    return;
                }
            }
            suitToUse = null;
        }

        private static void PickUnlockableBasedOnID(int suitID, out UnlockableItem itemToUse)
        {
            foreach (UnlockableSuit suit in allSuits)
            {
                if (suit.syncedSuitID.Value == suitID)
                {
                    itemToUse = UnlockableItems[suit.syncedSuitID.Value];
                    return;
                }
            }
            itemToUse = null;
        }

        private static void PickSuitBasedOnItem(UnlockableItem itemGiven, out UnlockableSuit suitToWear)
        {
            foreach(UnlockableSuit suit in allSuits)
            {
                if (UnlockableItems[suit.syncedSuitID.Value] == itemGiven)
                {
                    suitToWear = suit;
                    suitsTerminal.X("Found suit to wear");
                    return;
                }
            }
            suitToWear = null;
        }

        private static void FindSuitWithoutID(string suitName, List<int> dontUse)
        {
            List<UnlockableItem> duplicateNamedSuits = [];

            suitsTerminal.X("grabbing duplicate suits items to not use");
            foreach(int id in dontUse)
            {
                PickUnlockableBasedOnID(id, out UnlockableItem dontUseItem);
                if(dontUseItem != null)
                    duplicateNamedSuits.Add(dontUseItem);
            }

            suitsTerminal.X("iterating through item to grab the remaining suitID");
            foreach(UnlockableItem item in UnlockableItems)
            {
                if(item.unlockableName == suitName && !duplicateNamedSuits.Contains(item))
                {
                    suitsTerminal.X("Found unique suit, assigning.");
                    PickSuitBasedOnItem(item, out UnlockableSuit wearThis);
                    if(wearThis != null)
                    {
                        wearThis.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
                        suitsTerminal.X($"Wearing: {UnlockableItems[wearThis.syncedSuitID.Value].unlockableName}");
                    }
                    else
                        suitsTerminal.Log.LogError($"failed to get suit from item!");
                }
                        //{UnlockableItems[suit.syncedSuitID.Value].unlockableName}
            }
        }

        private static void DuplicateSuitHandling(string selectedSuit)
        {
            string numbersPortion = GetNumbers(selectedSuit);
            if (int.TryParse(numbersPortion, out int stringSuitID))
            {
                if(stringSuitID != -1)
                {
                    PickSuitBasedOnID(stringSuitID, out UnlockableSuit suit);
                    if (suit != null)
                    {
                        suit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
                        suitsTerminal.X($"Wearing: {UnlockableItems[suit.syncedSuitID.Value].unlockableName}");
                    }
                    else
                        suitsTerminal.Log.LogError($"failed to resolve suit name from {stringSuitID}");
                }
                else
                    suitsTerminal.Log.LogError($"failed to resolve suit name from {selectedSuit}");
            }
            else
                suitsTerminal.Log.LogError($"failed to resolve suit ID from {numbersPortion}");
        }

        internal static void AdvancedSuitPick(string selectedSuit)
        {
            string displayText;
            //suitsTerminal.X("1.");

            if (selectedSuit.Contains("^("))
            {
                DuplicateSuitHandling(selectedSuit);
                return;
            }

            if (UnlockableItems != null && selectedSuit != string.Empty)
            {
                //suitsTerminal.X("2.");
                foreach (UnlockableSuit suit in allSuits)
                {
                    if (suit.syncedSuitID != null && suit.syncedSuitID.Value >= 0)
                    {
                        suitsTerminal.X("3.");
                        if (UnlockableItems.Count < suit.syncedSuitID.Value)
                        {
                            suitsTerminal.Log.LogError("suit change encountered error with suitID");
                            return;
                        }

                        string SuitName = UnlockableItems[suit.syncedSuitID.Value].unlockableName;
                        if (SuitName.Equals(selectedSuit))
                        {
                            suitsTerminal.X("4.");
                            suit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
                            displayText = $"Changing suit to {UnlockableItems[suit.syncedSuitID.Value].unlockableName}\r\n";
                            suitsTerminal.X(displayText);
                            return;
                        }
                    }
                }
            }

            displayText = $"Unable to set suit to match command: {selectedSuit}";
            suitsTerminal.X(displayText);
            return;
        }

        internal static string PickSuit()
        {
            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {UnlockableItems.Count}");

            string cleanedText = GetScreenCleanedText(suitsTerminal.Terminal);
            string displayText;

            if (UnlockableItems != null)
            {
                foreach (UnlockableSuit suit in allSuits)
                {
                    if (suit.syncedSuitID.Value >= 0)
                    {
                        string SuitName = UnlockableItems[suit.syncedSuitID.Value].unlockableName.ToLower();
                        SuitName = TerminalFriendlyString(SuitName);
                        if (cleanedText.Equals("wear " + SuitName))
                        {
                            suit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
                            displayText = $"Changing suit to {UnlockableItems[suit.syncedSuitID.Value].unlockableName}\r\n";
                            return displayText;
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

            displayText = $"Unable to set suit to match command: {cleanedText}";
            return displayText;
        }

        private static string GetScreenCleanedText(Terminal __instance)
        {
            string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            return RemovePunctuation(s);
        }

        private static string RemovePunctuation(string s) //copied from game files
        {
            StringBuilder stringBuilder = new();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        internal static void AddCommand(bool clearText, string keyWord, string nodeName, Func<string> methodName, MainListing nodeListing, string category = "", string description = "")
        {
            TerminalNode newNode = AddingThings.AddNodeManual(nodeName, keyWord, methodName, clearText, 0, nodeListing);
            suitsTerminal.X($"{newNode.name} created!");
            if(category.ToLower() == "other")
            {
                TerminalNode otherNode = LogicHandling.GetFromAllNodes("OtherCommands");
                AddingThings.AddToExistingNodeText($"\n>{keyWord.ToUpper()}\n{description}", ref otherNode);
            }

        }

        internal static void AddBasicCommand(string nodeName, string keyWord, string displayText, string category = "", string description = "")
        {
            AddingThings.AddBasicCommand(nodeName, keyWord, displayText, false, true, category, description);
        }

    }
}
