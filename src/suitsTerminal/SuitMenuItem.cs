
using System.Collections.Generic;
using OpenLib.InteractiveMenus;
using static OpenLib.Events.Events;

namespace suitsTerminal;

internal class SuitMenuItem(string name, CustomEvent select = null!) : MenuItem(Menu.SuitsMenu)
{
    private string _name = name;
    public override string Name
    {
        get => _name;
        set => _name = value;
    }

    public bool _showEmpty = true;
    public override bool ShowIfEmptyNest
    {
        get => _showEmpty;
        set => _showEmpty = value;
    }

    private CustomEvent _selection = select;
    public override CustomEvent SelectionEvent
    {
        get
        {
            _selection ??= new();
            return _selection;
        }
        set => _selection = value;
    }

    private List<MenuItem> _nested = [];
    public override List<MenuItem> NestedMenus
    {
        get => _nested;
        set => _nested = value;
    }

    public SuitAttributes SuitProps = null!;

    internal static MenuItem GetStartMenu()
    {
        if (ModConfig.MenuStartPage.Value == ModConfig.Page.HelpPage)
            return Menu.HelpPage;
        else if (ModConfig.MenuStartPage.Value == ModConfig.Page.FavoritesListing)
            return Menu.FavoritesList;
        else if (ModConfig.MenuStartPage.Value == ModConfig.Page.SuitsListing)
            return Menu.SuitsList;

        return Menu.HomePage;
    }


}
