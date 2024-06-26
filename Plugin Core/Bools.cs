using System.Collections.Generic;
using static suitsTerminal.AllSuits;
using static suitsTerminal.Misc;

namespace suitsTerminal
{
    internal class Bools
    {
        internal static bool ShouldShowSuit(UnlockableSuit suit, List<string> suitNames)
        {
            if (SConfig.hideRack.Value)
                return false;

            if (!SConfig.suitsOnRackCustom.Value && normSuit >= SConfig.suitsOnRack.Value && !rackSituated)
                return false;

            if(showSuit == SConfig.suitsOnRack.Value && !rackSituated)
                return false;

            if (rackSituated && !isHanging)
                return false;

            if (SConfig.suitsOnRackCustom.Value && !suitNames.Contains(UnlockableItems[suit.syncedSuitID.Value].unlockableName))
                return false;

            if (rackSituated && isHanging)
                return true;

            if (showSuit < SConfig.suitsOnRack.Value && !rackSituated)
                return true;

            return false;
        }
    }
}
