using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Text;
using GameNetcodeStuff;
using static suitsTerminal.Misc;
using static suitsTerminal.AllSuits;
using TerminalApi;

namespace suitsTerminal
{
    
    public class Suits_Patch : MonoBehaviour
    {
        [HarmonyPatch(typeof(PlayerControllerB), "SpawnPlayerAnimation")]
        public class PlayerSpawnPatch : MonoBehaviour
        {
            static void Postfix()
            {
                if (!rackSituated)
                {
                    suitsTerminal.X("player loaded & rackSituated is false, fixing suits rack");
                    AdvancedMenu.InitSettings();
                    InitThisPlugin.InitSuitsTerm();
                    PictureInPicture.InitPiP();
                    hasLaunched = true;
                    return;
                }
                else
                    return;
            }
        }

        [HarmonyPatch(typeof(Terminal), "Awake")]
        public class AwakeTermPatch : Terminal
        {
            static void Postfix(Terminal __instance)
            {
                suitsTerminal.Terminal = __instance;
                suitsTerminal.X($"Setting suitsTerminal.Terminal");
                ResetVars();      
            }

            private static void ResetVars()
            {
                hasLaunched = false;
                normSuit = 0;
                showSuit = 0;
                hintOnce = false;
                rackSituated = false;
                favSuitsSet = false;
                PictureInPicture.PiPCreated = false;
                suitsTerminal.X("set initial suits values");
            }
        }

        [HarmonyPatch(typeof(Terminal), "LoadNewNodeIfAffordable")]
        public class AffordableNode : Terminal
        {
            static void Postfix(Terminal __instance)
            {
                suitsTerminal.X("purchase detected");
                InitThisPlugin.InitSuitsTerm();
            }
        }

        //RemoveLockedSuits


        [HarmonyPatch(typeof(StartOfRound), "Start")]
        public class StartofRoundPatch : MonoBehaviour
        {
            static void Postfix()
            {
                //old stuff was here
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
            if (__instance == null)
                return;

            if(hasLaunched)
            {
                suitsTerminal.X("suits rack func called, calling InitSuitsTerm func");
                InitThisPlugin.InitSuitsTerm();
            } 
                
        }
    }

    [HarmonyPatch(typeof(GameNetworkManager), "Start")]
    public class GameStartPatch
    {
        public static void Postfix()
        {
            //hasLaunched = false;
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