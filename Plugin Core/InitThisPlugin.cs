using static suitsTerminal.AllSuits;
using static suitsTerminal.Enums;
using static suitsTerminal.Misc;

namespace suitsTerminal
{
    internal class InitThisPlugin
    {
        internal static bool initStarted = false;

        internal static void InitSuitsTerm()
        {
            if (initStarted)
            {
                Plugin.X("init already started, ending func");
                return;
            }


            initStarted = true;
            Plugin.X($"Suits patch, showSuit value: {suitsOnRack}");
            InitSuitsListing();

            if (hintOnce)
                return;

            if (Plugin.Terminal == null)
            {
                Plugin.Log.LogError("~~ FATAL ERROR ~~");
                Plugin.Log.LogError("Terminal instance is NULL");
                Plugin.Log.LogError("~~ FATAL ERROR ~~");
                return;
            }
            Plugin.Terminal.StartCoroutine(ChatHints());
            Plugin.Terminal.StartCoroutine(HudHints());
            hintOnce = true;
        }
    }
}
