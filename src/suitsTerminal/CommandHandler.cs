using static suitsTerminal.RackManager;

namespace suitsTerminal;

internal class CommandHandler
{
    internal static string RandomSuit()
    {
        Loggers.LogDebug($"Suit Count: {AllSuits.Count}");

        string displayText;

        int random = Plugin.Rand.Next(AllSuits.Count);
        SuitAttributes suit = AllSuits[random];
        suit.WearSuit();
        displayText = $"========== Rolled random number [ {random} ] ==========\n\n\n\n\n\n\n\n\n\n\n\n\n\tChanging suit to {suit.Name}!\n\tPress any key to continue...\n\n";
        return displayText;
    }

    internal static string AdvancedSuitsTerm()
    {
        Menu.SuitsMenu.ExitAction = null!;
        RealCurrentID = Plugin.LocalPlayer.currentSuitID;
        Menu.SuitsMenu.EnterAtPage(SuitMenuItem.GetStartMenu());
        return "";
    }
}
