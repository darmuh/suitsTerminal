using BepInEx.Configuration;
using System.Collections.Generic;
using suitsTerminal.Interfaces;
using suitsTerminal.Suits;

namespace suitsTerminal.Util;

public static class Extensions
{
    public static void GetSuitAttributes(this UnlockableSuit suit)
    {
        if (suit == null)
            return;

        if (((IUnlockableSuit)suit).SuitsTerminalAttributes == null)
            ((IUnlockableSuit)suit).SuitsTerminalAttributes = suit.gameObject.AddComponent<SuitAttributes>();
    }

    public static void ResetPosition(this UnlockableSuit suit)
    {
        if (suit == null) return;

        RackManager.SuitSpawn(suit);
    }

    internal static List<string> GetKeywordValues(this ConfigEntry<string> entry, bool isMain = false)
    {
        if (string.IsNullOrEmpty(entry.Value))
        {
            if (isMain)
                return ["suits"];

            return [];
        }

        var entries = entry.Value.Split(',');
        List<string> result = [];

        foreach(var item in entries)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;
            else
                result.Add(item.Trim());
        }

        return result;
    }
}
