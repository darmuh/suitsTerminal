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
            Plugin.Terminal = instance;
            Plugin.X($"Setting suitsTerminal.Terminal");
            ResetVars();
        }
        private static void ResetVars()
        {
            hasLaunched = false;
            suitsOnRack = 0;
            hintOnce = false;
            rackSituated = false;
            PictureInPicture.PiPCreated = false;
            Plugin.X("set initial variables");

            if(resetSuitPlacementOnRestart)
            {
                ResetSuitPlacementVars(); //rack settings config change
            }
        }

        private static void ResetSuitPlacementVars()
        {
            if (suitListing.SuitsList.Count == 0)
                return;

            suitListing.SuitsList.ForEach(s => s.IsOnRack = false);
            resetSuitPlacementOnRestart = false;
        }

        internal static void OnGameStart()
        {
            CompatibilityCheck();
            InitFavoritesListing();
        }

        private static void CompatibilityCheck()
        {
            if (SoftCompatibility("darmuh.TerminalStuff", ref Plugin.TerminalStuff))
            {
                Plugin.X("darmuhsTerminalStuff compatibility enabled!");
            }
            if (SoftCompatibility("Hexnet.lethalcompany.suitsaver", ref Plugin.SuitSaver))
            {
                Plugin.X("Suitsaver compatibility enabled!\nDefaultSuit will not be loaded");
            }
        }

        private static void DefaultSuit()
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
            if (!rackSituated)
            {
                Plugin.X("player loaded & rackSituated is false, fixing suits rack");
                PiPStuff();
                AdvancedMenu.InitSettings();
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
