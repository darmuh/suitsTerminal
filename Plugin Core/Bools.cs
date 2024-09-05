using System.Collections.Generic;
using static suitsTerminal.AllSuits;
using static suitsTerminal.Misc;
using static OpenLib.Common.CommonStringStuff;

namespace suitsTerminal
{
    internal class Bools
    {
        internal static bool ShouldShowSuit(UnlockableSuit suit, List<string> suitNames)
        {
            List<string> suitsOnRackOnly = GetListToLower(suitNames);
            List<string> neverAddToRack = GetListToLower(GetKeywordsPerConfigItem(SConfig.DontAddToRack.Value, ','));

            if (SConfig.HideRack.Value)
                return false;

            if (neverAddToRack.Contains(UnlockableItems[suit.syncedSuitID.Value].unlockableName.ToLower()))
                return false;

            if (suitsOnRackOnly.Count == 0 && showSuit >= SConfig.SuitsOnRack.Value && !rackSituated)
                return false;

            if(showSuit == SConfig.SuitsOnRack.Value && !rackSituated)
                return false;

            if (rackSituated && !isHanging)
                return false;

            if (suitsOnRackOnly.Count > 0 && !suitsOnRackOnly.Contains(UnlockableItems[suit.syncedSuitID.Value].unlockableName.ToLower()))
                return false;

            if (rackSituated && isHanging)
                return true;

            if (showSuit < SConfig.SuitsOnRack.Value && !rackSituated)
                return true;

            return false;
        }
    }
}
