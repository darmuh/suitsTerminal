using OpenLib.Common;
using OpenLib.Events;
using static suitsTerminal.Misc;
using static suitsTerminal.StringStuff;
using static suitsTerminal.AllSuits;

namespace suitsTerminal.EventSub
{
    internal class Subscribers
    {
        internal static void Subscribe()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            EventManager.TerminalLoadIfAffordable.AddListener(TerminalGeneral.OnLoadAffordable);

            //Unique
            //EventManager.GetNewDisplayText.AddListener(TerminalParse.OnNewDisplayText);
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
    }
}
