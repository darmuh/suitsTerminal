using OpenLib.ConfigManager;
using suitsTerminal.Suit_Stuff;
using System.Collections.Generic;
using System.Text;
using static suitsTerminal.AllSuits;
using static suitsTerminal.Misc;
using static suitsTerminal.StringStuff;

namespace suitsTerminal
{
    internal class OldCommands
    {
        internal static void CreateOldWearCommands(SuitAttributes suit, ref List<string> suitNames)
        {
            if (!SConfig.TerminalCommands.Value)
                return;

            if (SConfig.AdvancedTerminalMenu.Value)
                return;

            if (suit.HideFromTerminal)
                return;

            //AddCommand(string textFail, bool clearText, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)

            if (suit.Suit.syncedSuitID.Value >= 0 && !keywordsCreated)
            {
                suit.Name = TerminalFriendlyString(suit.Name);
                if (suitNames.Contains(suit.Name.ToLower()))
                {
                    suit.Name += "z";
                    Plugin.WARNING($"Duplicate found. Updated SuitName: {suit.Name}");
                }
                suitNames.Add(suit.Name.ToLower());
                CommandHandler.AddCommand(true, "wear " + suit.Name, suit.Name, CommandHandler.SuitPickCommand, ConfigSetup.defaultListing);
                Plugin.X($"Keyword for {suit.Name} added");
            }
            else if (suit.Suit.syncedSuitID.Value >= 0 && keywordsCreated)
            {
                suit.Name = TerminalFriendlyString(suit.Name);
                CommandHandler.AddCommand(true, "wear " + suit.Name, suit.Name, CommandHandler.SuitPickCommand, ConfigSetup.defaultListing);
                Plugin.X($"Keyword for {suit.Name} updated");

            }
            else if (suit.Suit.syncedSuitID.Value < 0)
            {
                weirdSuitNum++;
                Plugin.WARNING($"Skipping suit with invalid ID number: {suit.Suit.syncedSuitID.Value}");
            }
            else
            {
                Plugin.WARNING($"Unexpected condition in CreateOldWearCommands!");
            }
        }

        internal static void CreateOldPageCommands()
        {
            if (!SConfig.TerminalCommands.Value)
                return;

            if (SConfig.AdvancedTerminalMenu.Value)
                return;

            if (Plugin.Terminal != null && !keywordsCreated)
            {
                StringBuilder suitsList = BuildSuitsList();

                CreateSuitPages(suitsList);

                keywordsCreated = true;
            }
            else if (keywordsCreated)
            {
                StringBuilder suitsList = BuildSuitsList();

                UpdateOrRecreateSuitKeywords(suitsList);
            }
        }

        private static StringBuilder BuildSuitsList()
        {
            StringBuilder suitsList = new();
            weirdSuitNum = 0;

            foreach (SuitAttributes item in suitListing.SuitsList)
            {
                if (item.Suit == null)
                {
                    Plugin.WARNING($"{item.Name} contains NULL suit ref");
                    continue;
                }

                if (item.HideFromTerminal)
                    continue;

                string SuitName;
                if (item.Suit.syncedSuitID.Value >= 0)
                {
                    SuitName = item.Name;
                    SuitName = TerminalFriendlyString(SuitName);
                    suitsList.AppendLine($"> wear {SuitName}\n");
                }
                else
                {
                    weirdSuitNum++;
                    Plugin.X($"Skipping suit.");
                }
            }

            Plugin.X($"Full Suit List: {suitsList}");
            return suitsList;
        }

        private static void CreateSuitPages(StringBuilder suitsList)
        {
            List<Page> pages = PageSplitter.SplitTextIntoPages(suitsList.ToString(), 6);

            if (SConfig.TerminalCommands.Value)
            {
                foreach (var page in pages)
                {
                    if (page.PageNumber == 1)
                    {
                        CreateOrUpdateMainSuitCommand(page);
                    }
                    else
                    {
                        CreateOrUpdateSuitPage(page);
                    }
                }
            }
        }

        private static void UpdateOrRecreateSuitKeywords(StringBuilder suitsList)
        {
            List<Page> pages = PageSplitter.SplitTextIntoPages(suitsList.ToString(), 6);

            if (SConfig.TerminalCommands.Value)
            {
                foreach (var page in pages)
                {
                    if (page.PageNumber == 1)
                    {
                        CreateOrUpdateMainSuitCommand(page);
                    }
                    else
                    {
                        CreateOrUpdateSuitPage(page);
                    }
                }
            }
        }

        // Methods for creating or updating suit commands and keywords

        // Helper method to create or update the main suit command
        private static void CreateOrUpdateMainSuitCommand(Page page)
        {
            CommandHandler.AddBasicCommand("suits (main)", "suits", $"{page.Content}", "other", "Display a listing of available suits to wear");
            Plugin.X($"Updating main suits command");
        }

        // Helper method to create or update suit pages
        private static void CreateOrUpdateSuitPage(Page page)
        {
            Plugin.X($"Creating page {page.PageNumber} keyword");
            CommandHandler.AddBasicCommand("suits (pg.{page.PageNumber})", $"suits {page.PageNumber}", $"{page.Content}");
            //Plugin.X($"Created keyword 'suits {page.PageNumber}'");
        }

        internal static void MakeRandomSuitCommand()
        {
            if (!SConfig.RandomSuitCommand.Value)
                return;

            CommandHandler.AddCommand(true, "randomsuit", "random suit command", CommandHandler.RandomSuit, ConfigSetup.defaultListing, "other", "Equip a random suit");

        }
    }
}
