using suitsTerminal.Suit_Stuff;
using System.Collections.Generic;
using System.Linq;
using static OpenLib.Common.CommonStringStuff;
using static suitsTerminal.Misc;

namespace suitsTerminal
{
    internal class Bools
    {
        internal static bool ShouldShowSuit(SuitAttributes suit)
        {
            List<string> suitsOnRackOnly = GetKeywordsPerConfigItem(SConfig.SuitsOnRackOnly.Value, ',');
            List<string> neverAddToRack = GetListToLower(GetKeywordsPerConfigItem(SConfig.DontAddToRack.Value, ','));

            if (SConfig.HideRack.Value)
                return false;

            if (neverAddToRack.Contains(suit.Name.ToLower()))
                return false;

            if (suitsOnRackOnly.Count == 0 && suitsOnRack >= SConfig.SuitsOnRack.Value && !rackSituated)
                return false;

            if (suitsOnRack == SConfig.SuitsOnRack.Value && !rackSituated)
                return false;

            if (rackSituated && !suit.IsOnRack)
                return false;

            if (suitsOnRackOnly.Count > 0 && !suitsOnRackOnly.Any(s => s.ToLower() == suit.Name.ToLower()))
                return false;

            if (rackSituated && suit.IsOnRack)
                return true;

            if (suitsOnRack < SConfig.SuitsOnRack.Value && !rackSituated)
                return true;

            return false;
        }
    }
}
