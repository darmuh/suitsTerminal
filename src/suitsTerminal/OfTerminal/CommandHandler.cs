using suitsTerminal.Suits;
using suitsTerminal.Util;
using static suitsTerminal.OfTerminal.Menu;
using static suitsTerminal.Suits.RackManager;

namespace suitsTerminal.OfTerminal;

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

    private static void EnteringMenu()
    {
        SuitsMenu.ExitAction = null!;
        RealCurrentID = Plugin.LocalPlayer.currentSuitID;
    }

    internal static string MainSuitsMenu()
    {
        HomePage.AddNestedItem(FavoritesList);
        HomePage.AddNestedItem(SuitsList);
        SuitsMenu.MenuNode = Main.terminalNode;
        EnteringMenu();
        SuitsMenu.EnterAtPage(SuitMenuItem.GetStartMenu());
        return "";
    }

    internal static string FavsSuitsMenu()
    {
        FavoritesList.RemoveFromParent();
        SuitsMenu.MenuNode = FavListing.terminalNode;
        EnteringMenu();
        SuitsMenu.EnterAtPage(FavoritesList);
        return "";
    }


    internal static string SuitsListMenu()
    {
        SuitsList.RemoveFromParent();
        SuitsMenu.MenuNode = SuitListing.terminalNode;
        EnteringMenu();
        SuitsMenu.EnterAtPage(SuitsList);
        return "";
    }
}
