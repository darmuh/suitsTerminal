using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalApi.Classes;
using static suitsTerminal.AllSuits;
using static suitsTerminal.StringStuff;
using static TerminalApi.TerminalApi;

namespace suitsTerminal
{
    internal class CommandHandler
    {
        public static List<CommandInfo> suitsTerminalCommands = new List<CommandInfo>();
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
                        randomSuit.SwitchSuitToThis();
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

        internal static void SuitPickCommand(out string displayText)
        {
            displayText = PickSuit();
            return;
        }

        internal static void RandomSuitCommand(out string displayText)
        {
            displayText = RandomSuit();
            return;
        }

        internal static void AdvancedSuitsTerm(out string displayText)
        {
            suitsTerminal.Terminal.StartCoroutine(AdvancedMenu.ActiveMenu());
            displayText = AdvancedMenuDisplay(suitNames, 0, 10, 1);
            return;
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
            List<UnlockableItem> duplicateNamedSuits = new List<UnlockableItem>();

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
                        wearThis.SwitchSuitToThis();
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
                        suit.SwitchSuitToThis();
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
            string displayText = string.Empty;
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
            string displayText = string.Empty;

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
                            suit.SwitchSuitToThis();
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

        internal static void AddCommand(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
        {
            if (GetKeyword(keyWord) != null)
                return;

            TerminalNode node = CreateTerminalNode(textFail, clearText);
            TerminalKeyword termWord = CreateTerminalKeyword(keyWord, isVerb, node);

            CommandInfo commandInfo = new CommandInfo()
            {
                TriggerNode = node,
                DisplayTextSupplier = () =>
                {
                    methodName(out string displayText);
                    return displayText;
                },
                Category = category,
                Description = description
            };
            suitsTerminalCommands.Add(commandInfo);

            AddTerminalKeyword(termWord, commandInfo);

            node.name = nodeName;
            nodeGroup.Add(node);
        }

    }
}
