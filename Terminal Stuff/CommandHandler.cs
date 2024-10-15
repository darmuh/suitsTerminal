using OpenLib.CoreMethods;
using suitsTerminal.Suit_Stuff;
using System;
using static OpenLib.Common.CommonStringStuff;
using static suitsTerminal.AllSuits;
using static suitsTerminal.StringStuff;
using Random = System.Random;

namespace suitsTerminal
{
    internal class CommandHandler
    {
        internal static string RandomSuit()
        {
            Plugin.X($"Suit Count: {suitListing.SuitsList.Count}");

            string displayText;

            Random rand = new();
            int random = rand.Next(suitListing.SuitsList.Count);
            SuitAttributes suit = suitListing.SuitsList[random];
            suit.Suit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
            displayText = $"Rolled random number [ {random} ]\n\n\nChanging suit to {suit.Name}!\r\n\r\n";
            return displayText;
        }

        internal static string SuitPickCommand()
        {
            string displayText = PickSuit();
            return displayText;
        }

        internal static string AdvancedSuitsTerm()
        {
            Plugin.Terminal.StartCoroutine(AdvancedMenu.ActiveMenu());
            string displayText = AdvancedMenuDisplay(suitListing, 0, 10, 1);
            return displayText;
        }

        internal static void BetterSuitPick(SuitAttributes suit)
        {
            if (suit.Suit == null)
            {
                Plugin.ERROR("suit is null!");
                return;
            }

            suit.Suit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
            Plugin.Log.LogMessage($"Switched suit to {suit.Name}");
        }

        internal static string PickSuit()
        {
            Plugin.X($"Suit Count: {suitListing.SuitsList.Count}");
            Plugin.X($"Unlockables Count: {UnlockableItems.Count}");

            string cleanedText = GetCleanedScreenText(Plugin.Terminal).ToLower();
            string cleanName;
            string displayText;

            foreach (SuitAttributes suit in suitListing.SuitsList)
            {
                if (suit.Suit.syncedSuitID.Value >= 0)
                {
                    cleanName = TerminalFriendlyString(suit.Name);
                    if (cleanedText.Equals("wear " + cleanName))
                    {
                        suit.Suit.SwitchSuitToThis(StartOfRound.Instance.localPlayerController);
                        displayText = $"Changing suit to {suit.Name}\r\n";
                        return displayText;
                    }
                    else
                        Plugin.X($"SuitName: {suit.Name} doesn't match Cleaned Text: {cleanedText}");
                }
                else
                {
                    Plugin.X($"suit ID was {suit.Suit.syncedSuitID.Value}");
                }
            }

            displayText = $"Unable to set suit to match command: {cleanedText}";
            return displayText;
        }

        internal static void AddCommand(bool clearText, string keyWord, string nodeName, Func<string> methodName, MainListing nodeListing, string category = "", string description = "")
        {
            TerminalNode newNode = AddingThings.AddNodeManual(nodeName, keyWord, methodName, clearText, 0, nodeListing);
            if (category.ToLower() == "other")
            {
                if (LogicHandling.TryGetFromAllNodes("OtherCommands", out TerminalNode otherNode))
                {
                    AddingThings.AddToExistingNodeText($"\n>{keyWord.ToUpper()}\n{description}", ref otherNode);
                }
            }

        }

        internal static void AddBasicCommand(string nodeName, string keyWord, string displayText, string category = "", string description = "")
        {
            AddingThings.AddBasicCommand(nodeName, keyWord, displayText, false, true, category, description);
        }

    }
}
