using OpenLib.Events;
using static OpenLib.Common.StartGame;
using static OpenLib.Common.CommonStringStuff;
using static suitsTerminal.Misc;
using static suitsTerminal.AllSuits;
using System.Collections.Generic;
using OpenBodyCams;
using UnityEngine;
using System.Collections;

namespace suitsTerminal.EventSub
{
    internal class Subscribers
    {
        internal static void Subscribe()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            EventManager.TerminalDelayStart.AddListener(SetDefaultSuit);
            EventManager.TerminalLoadIfAffordable.AddListener(TerminalGeneral.OnLoadAffordable);
            EventManager.GameNetworkManagerStart.AddListener(OnGameStart);
            EventManager.PlayerSpawn.AddListener(OnPlayerSpawn);

            //Unique
            //EventManager.GetNewDisplayText.AddListener(TerminalParse.OnNewDisplayText);
        }

        internal static void SetDefaultSuit()
        {
            DefaultSuit();
        }

        internal static void OnTerminalAwake(Terminal instance)
        {
            suitsTerminal.Terminal = instance;
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
            suitsTerminal.X("set initial variables");
        }

        internal static void OnGameStart()
        {
            CompatibilityCheck();
        }

        private static void CompatibilityCheck()
        {
            if (SoftCompatibility("Zaggy1024.OpenBodyCams", ref suitsTerminal.OpenBodyCams))
            {
                suitsTerminal.X("OpenBodyCams compatibility enabled!");
            }
            if (SoftCompatibility("darmuh.TerminalStuff", ref suitsTerminal.TerminalStuff))
            {
                suitsTerminal.X("darmuhsTerminalStuff compatibility enabled!");
            }
            if(SoftCompatibility("Hexnet.lethalcompany.suitsaver", ref suitsTerminal.SuitSaver))
            {
                suitsTerminal.X("Suitsaver compatibility enabled!\nDefaultSuit will not be loaded");
            }
        }

        private static void DefaultSuit()
        {
            if (SConfig.DefaultSuit.Value.Length < 1 || SConfig.DefaultSuit.Value.ToLower() == "default")
                return;

            if(suitsTerminal.SuitSaver)
            {
                suitsTerminal.WARNING("Suitsaver detected, default suit will not be loaded.");
                return;
            }

            List<string> lowerCaseSuits = GetListToLower(suitNames);
            if (lowerCaseSuits.Contains(SConfig.DefaultSuit.Value.ToLower()))
            {
                suitsTerminal.X($"Setting default suit: {SConfig.DefaultSuit.Value}");
                CommandHandler.AdvancedSuitPick(SConfig.DefaultSuit.Value);
            }
            else
                suitsTerminal.WARNING($"Could not set default suit to {SConfig.DefaultSuit.Value}");
        }

        internal static void OnPlayerSpawn()
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
}
