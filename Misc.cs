using System;
using System.Collections.Generic;
using System.Text;
using TerminalApi;
using static suitsTerminal.AdvancedMenu;
using static suitsTerminal.AllSuits;

namespace suitsTerminal
{
    internal class Misc
    {
        internal static bool keywordsCreated = false;
        internal static bool rackSituated = false;
        internal static int normSuit = 0;
        internal static int showSuit = 0;
        internal static int weirdSuitNum = 0;
        internal static int reorderSuits = 0;
        internal static bool hasLaunched = false;
        internal static bool hintOnce = false;
        internal static bool isHanging = false;

        internal static List<string> GetString()
        {
            if(favSuits.Count == 0)
            {
                suitsTerminal.Log.LogWarning("Favorite suits empty, displaying regular menu");
                inFavsMenu = false;
                return suitNames;
            }

            if (inFavsMenu)
                return favSuits;
            else
                return suitNames;
        }

        internal static void SaveToConfig(List<string> stringList, out string configItem)
        {
            configItem = string.Join(", ", stringList);
            suitsTerminal.X($"Saving to config\n{configItem}");
        }
    }
}
