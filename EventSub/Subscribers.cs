using OpenLib.Events;
using suitsTerminal.Suit_Stuff;
using System.Linq;
using static OpenLib.Common.StartGame;
using static suitsTerminal.AllSuits;
using static suitsTerminal.Misc;

namespace suitsTerminal.EventSub
{
    internal class Subscribers
    {
        internal static void Subscribe()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            //EventManager.TerminalQuit.AddListener(OnTerminalQuit);
            EventManager.TerminalDisable.AddListener(OnTerminalDisable);
            EventManager.TerminalDelayStart.AddListener(OnTerminalDelayStart);
            EventManager.TerminalLoadIfAffordable.AddListener(TerminalGeneral.OnLoadAffordable);
            EventManager.GameNetworkManagerStart.AddListener(OnGameStart);
            EventManager.PlayerSpawn.AddListener(OnPlayerSpawn);
            EventManager.ShipReset.AddListener(OnShipReset);

            //Unique
            OpenLib.TerminalUpdatePatch.usePatch = true; //needed for below event listener
            //EventManager.TerminalKeyPressed.AddListener(AdvancedMenu.OnTerminalKeyPress);
            
        }

        internal static void OnTerminalDelayStart()
        {
            DefaultSuit();
        }

        internal static void OnTerminalAwake(Terminal instance)
        {
            Plugin.Terminal = instance;
            Plugin.X($"Setting suitsTerminal.Terminal");
        }

        internal static void OnShipReset()
        {
            StartOfRound.Instance.StartCoroutine(Enums.DelayFixRack());
        }

        internal static void OnTerminalDisable()
        {
            suitsOnRack = 0;
            hasLaunched = false;
            hintOnce = false;
            rackSituated = false;
            PictureInPicture.PiPCreated = false;
            AdvancedMenu.specialMenusActive = false;
            Plugin.X("set initial variables");
            ResetSuitPlacementVars(true);
        }

        internal static void ResetSuitPlacementVars(bool unlocksReset)
        {
            if (suitListing.SuitsList.Count == 0)
                return;

            if (unlocksReset)
            {
                suitListing.ClearAll();
                Plugin.X("suitlisting cleared!");
                return;
            }

            suitListing.SuitsList.ForEach(s => s.IsOnRack = false);

            rackSituated = false;
        }

        internal static void OnGameStart()
        {
            CompatibilityCheck();
        }

        private static void CompatibilityCheck()
        {
            Plugin.X("Compatibility Check!");

            //if (SoftCompatibility("darmuh.TerminalStuff", ref Plugin.TerminalStuff))
                //Plugin.X("darmuhsTerminalStuff compatibility enabled!");

            if (SoftCompatibility("Hexnet.lethalcompany.suitsaver", ref Plugin.SuitSaver))
                Plugin.X("Suitsaver compatibility enabled!\nDefaultSuit will not be loaded");

            if (SoftCompatibility("TooManySuits", ref Plugin.TooManySuits))
                Plugin.X("TooManySuits Compatibility enabled!\nRack will be left untouched!");
                
        }

        internal static void DefaultSuit()
        {
            if (SConfig.DefaultSuit.Value.Length < 1 || SConfig.DefaultSuit.Value.ToLower() == "default")
                return;

            if (Plugin.SuitSaver)
            {
                Plugin.WARNING("Suitsaver detected, default suit will not be loaded.");
                return;
            }

            if (suitListing.SuitsList.Any(x => x.Name.ToLower() == SConfig.DefaultSuit.Value.ToLower()))
            {
                SuitAttributes suit = suitListing.SuitsList.Find(x => x.Name.ToLower() == SConfig.DefaultSuit.Value.ToLower());
                CommandHandler.BetterSuitPick(suit);
            }
            else
                Plugin.WARNING($"Could not set default suit to {SConfig.DefaultSuit.Value}");
        }

        internal static void OnPlayerSpawn()
        {
            //AdvancedMenu.CaretOriginal = Plugin.Terminal.screenText.caretColor;
            if (!rackSituated)
            {
                Plugin.X("player loaded & rackSituated is false, fixing suits rack");
                PiPStuff();
                AdvancedMenu.InitBetterMenu();
                InitThisPlugin.InitSuitsTerm();
                PictureInPicture.InitPiP();
                hasLaunched = true;
                return;
            }
            else
                return;
        }

        private static void PiPStuff()
        {
            PictureInPicture.shadowDefault = StartOfRound.Instance.localPlayerController.thisPlayerModel.shadowCastingMode;
            PictureInPicture.modelLayerDefault = StartOfRound.Instance.localPlayerController.thisPlayerModel.gameObject.layer;
        }
    }
}
