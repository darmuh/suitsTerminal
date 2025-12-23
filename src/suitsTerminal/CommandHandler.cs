using System;
using HarmonyLib;
using OpenLib.CoreMethods;
using static suitsTerminal.RackManager;

namespace suitsTerminal;

internal class CommandHandler
{
    internal static string RandomSuit()
    {
        Loggers.LogDebug($"Suit Count: {AllSuits.Count}");

        string displayText;

        Random rand = new();
        int random = rand.Next(AllSuits.Count);
        SuitAttributes suit = AllSuits[random];
        suit.Suit.SwitchSuitToThis(Plugin.LocalPlayer);
        displayText = $"Rolled random number [ {random} ]\n\n\nChanging suit to {suit.Name}!\r\n\r\n";
        return displayText;
    }

    internal static string AdvancedSuitsTerm()
    {
        Menu.SuitsMenu.ExitAction = null!;
        RealCurrentID = Plugin.LocalPlayer.currentSuitID;
        Menu.SuitsMenu.EnterAtPage(SuitMenuItem.GetStartMenu());
        return "";
    }

    internal static void BetterSuitPick(SuitAttributes suit)
    {
        if (suit.Suit == null)
        {
            Loggers.ERROR("suit is null!");
            return;
        }

        Plugin.LocalPlayer.currentSuitID = RealCurrentID; //needed to sync new changes and play the sound
        suit.Suit.SwitchSuitToThis(Plugin.LocalPlayer);
        Plugin.Log.LogMessage($"Switched suit to {suit.Name}");
        RealCurrentID = suit.Suit.suitID;
    }

    internal static void AddBasicCommand(string nodeName, string keyWord, string displayText, string category = "", string description = "")
    {
        AddingThings.AddBasicCommand(nodeName, keyWord, displayText, false, true, category, description);
    }
}
