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
                suitsTerminal.X("init already started, ending func");
                return;
            }
               

            initStarted = true;
            suitsTerminal.X($"Suits patch, normSuit value: {normSuit} & showSuit value: {showSuit}");
            InitSuitsListing();

            if (hintOnce)
                return;

            suitsTerminal.Terminal.StartCoroutine(ChatHints());
            suitsTerminal.Terminal.StartCoroutine(HudHints());
            hintOnce = true;
        }
    }
}
