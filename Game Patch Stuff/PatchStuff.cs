using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using static suitsTerminal.Misc;

namespace suitsTerminal
{

    [HarmonyPatch(typeof(StartOfRound), "PositionSuitsOnRack")]
    public class SuitBoughtByOthersPatch
    {
        static void Postfix(StartOfRound __instance)
        {
            if (__instance == null)
                return;

            if (hasLaunched)
            {
                Plugin.X("suits rack func called, calling InitSuitsTerm func");
                InitThisPlugin.InitSuitsTerm();
            }

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

            List<Page> pages = [];
            int lineNumber = 0;
            int pageNumber = 1;

            while (lineNumber < lines.Length)
            {
                Page page = new() { Content = new StringBuilder(), PageNumber = pageNumber };

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