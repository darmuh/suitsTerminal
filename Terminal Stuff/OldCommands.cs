using Steamworks.Ugc;
using System;
using static suitsTerminal.StringStuff;
using static suitsTerminal.Misc;
using static suitsTerminal.AllSuits;
using static suitsTerminal.TerminalHook;
using System.Collections.Generic;
using System.Text;

namespace suitsTerminal
{
    internal class OldCommands
    {
        internal static void CreateOldWearCommands(UnlockableSuit item)
        {
            if (SConfig.terminalCommands.Value && !SConfig.advancedTerminalMenu.Value)
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
                        suitsTerminal.X($"Duplicate found. Updated SuitName: {SuitName}");
                    }
                    suitNames.Add(SuitName.ToLower());
                    CommandHandler.AddCommand(SuitName, true, "wear " + SuitName, false, SuitName, CommandHandler.SuitPickCommand, CommandStuff.sT);
                    suitsTerminal.X($"Keyword for {SuitName} added");
                }
                else if (item.syncedSuitID.Value >= 0 && keywordsCreated)
                {
                    SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;
                    SuitName = TerminalFriendlyString(SuitName);
                    if (TryGetKeyword("wear " + SuitName))
                    {
                        CommandHandler.AddCommand(SuitName, true, "wear " + SuitName, false, SuitName, CommandHandler.SuitPickCommand, CommandStuff.sT);
                        suitsTerminal.X($"Keyword for {SuitName} updated");
                    }
                    else
                    {
                        CommandHandler.AddCommand(SuitName, true, "wear " + SuitName, false, SuitName, CommandHandler.SuitPickCommand, CommandStuff.sT);
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

            if (SConfig.terminalCommands.Value)
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

            if (SConfig.terminalCommands.Value)
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
            if (TryGetKeyword("suits"))
            {
                //MakeCommand("Quit Terminal", keyword, "", false, true, QuitTerminalCommand, darmuhsTerminalStuff);
                MakeCommand("suits (main)", "suits", $"{page.Content}", false, true);
                suitsTerminal.X($"Updating main suits command");
            }
            else
            {
                MakeCommand("suits (main)", "suits", $"{page.Content}", false, true);
                suitsTerminal.X($"main suits command was deleted, creating again");
            }
        }

        // Helper method to create or update suit pages
        private static void CreateOrUpdateSuitPage(Page page)
        {
            if (TryGetKeyword($"suits {page.PageNumber}"))
            {
                suitsTerminal.X($"page {page.PageNumber} has already been creating, updating keyword");
                MakeCommand($"suits (pg.{page.PageNumber})", $"suits {page.PageNumber}", $"{page.Content}", false, true);
                //suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
            }
            else
            {
                suitsTerminal.X($"page {page.PageNumber} has been deleted, creating keyword again");
                MakeCommand($"suits (pg.{page.PageNumber})", $"suits {page.PageNumber}", $"{page.Content}", false, true);
                //suitsTerminal.X($"Created keyword 'suits {page.PageNumber}'");
            }
        }

        internal static void MakeRandomSuitCommand()
        {
            if (!SConfig.randomSuitCommand.Value)
                return;
            
                CommandHandler.AddCommand("random suit command", true, "randomsuit", false, "sT_random_suit", CommandHandler.RandomSuitCommand, CommandStuff.sT);
        }
    }
}
