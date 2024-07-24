using System;
using System.Collections.Generic;
using System.Text;

namespace suitsTerminal.EventSub
{
    internal class TerminalGeneral
    {
        internal static void OnLoadAffordable(TerminalNode node)
        {
            if (node.shipUnlockableID < 0 || node.shipUnlockableID > StartOfRound.Instance.unlockablesList.unlockables.Count)
                return;

            if (StartOfRound.Instance.unlockablesList.unlockables[node.shipUnlockableID].unlockableType == 0) //suit purchase detected
            {
                suitsTerminal.X("suit purchase detected, refreshing suitsTerminal");
                InitThisPlugin.InitSuitsTerm();
            }
        }
    }
}
