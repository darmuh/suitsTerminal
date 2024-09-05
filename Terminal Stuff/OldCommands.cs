using static suitsTerminal.StringStuff;
using static suitsTerminal.Misc;
using static suitsTerminal.AllSuits;
using System.Collections.Generic;
using System.Text;
using OpenLib.ConfigManager;

namespace suitsTerminal
{
    internal class OldCommands
    {
        internal static void CreateOldWearCommands(UnlockableSuit item, List<string> dontAddTerminal)
        {

            if (dontAddTerminal.Contains(UnlockableItems[item.syncedSuitID.Value].unlockableName.ToLower()))
                return;

            if (SConfig.TerminalCommands.Value && !SConfig.AdvancedTerminalMenu.Value)
            {
                //AddCommand(string textFail, bool clearText, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
                string SuitName = "";
                if (item.syncedSuitID.Value >= 0 && !keywordsCreated)
                {
                    SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;
                    SuitName = TerminalFriendlyString(SuitName);
                    if (suitNames.Contains(SuitName.ToLower()))
                    {
                        SuitName += "z";
                        suitsTerminal.WARNING($"Duplicate found. Updated SuitName: {SuitName}");
                    }
                    suitNames.Add(SuitName.ToLower());
                    CommandHandler.AddCommand(true, "wear " + SuitName, SuitName, CommandHandler.SuitPickCommand, ConfigSetup.defaultListing);
                    suitsTerminal.X($"Keyword for {SuitName} added");
                }
                else if (item.syncedSuitID.Value >= 0 && keywordsCreated)
                {
                    SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;
                    SuitName = TerminalFriendlyString(SuitName);
                    CommandHandler.AddCommand(true, "wear " + SuitName, SuitName, CommandHandler.SuitPickCommand, ConfigSetup.defaultListing);
                    suitsTerminal.X($"Keyword for {SuitName} updated");

                }
                else if (item.syncedSuitID.Value < 0)
                {
                    weirdSuitNum++;
                    suitsTerminal.WARNING($"Skipping suit with invalid ID number: {item.syncedSuitID.Value}");
                }
                else
                {
                    suitsTerminal.WARNING($"Unexpected condition in CreateOldWearCommands!");
                }

                CreateOldPageCommands();
            }
        }

        internal static void CreateOldPageCommands()
        {
            if (suitsTerminal.Terminal != null && !keywordsCreated)
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

            foreach (UnlockableSuit item in allSuits)
            {
                string SuitName;
                if (item.syncedSuitID.Value >= 0)
                {
                    SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;
                    SuitName = TerminalFriendlyString(SuitName);
                    suitsList.AppendLine($"> wear {SuitName}\n");
                }
                else
                {
                    weirdSuitNum++;
                    suitsTerminal.X($"Skipping suit.");
                }
            }
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
                suitsTerminal.X($"Updating main suits command");
        }

        // Helper method to create or update suit pages
        private static void CreateOrUpdateSuitPage(Page page)
        {
            suitsTerminal.X($"Creating page {page.PageNumber} keyword");
            CommandHandler.AddBasicCommand("suits (pg.{page.PageNumber})", $"suits {page.PageNumber}", $"{page.Content}");
            //suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
        }

        internal static void MakeRandomSuitCommand()
        {
            if (!SConfig.RandomSuitCommand.Value)
                return;
            
            CommandHandler.AddCommand(true, "randomsuit", "random suit command", CommandHandler.RandomSuit, ConfigSetup.defaultListing, "other", "Equip a random suit");

        }
    }
}
