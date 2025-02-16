using OpenLib.InteractiveMenus;
using System.Collections.Generic;
using static OpenLib.Events.Events;
using static suitsTerminal.AdvancedMenu;

namespace suitsTerminal
{
    internal class SuitMenuItem(string name, CustomEvent select = null!) : MenuItem
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
            get => _selection;
            set => _selection = value;
        }

        private List<MenuItem> _nested = [];
        public override List<MenuItem> NestedMenus
        {
            get => _nested;
            set => _nested = value;
        }

        public void AddToBetterMenu()
        {
            if (suitsMenu == null)
                return;

            if (suitsMenu.AllMenuItemsOfType == null)
                return;

            if(!suitsMenu.AllMenuItemsOfType.Contains(this))
                suitsMenu.AllMenuItemsOfType.Add(this);
        }

        public static void AddListToBetterMenu(List<SuitMenuItem> menuList)
        {
            suitsMenu.AllMenuItemsOfType.AddRange(menuList);
        }

        internal static MenuItem GetStartMenu()
        {
            //"main", "favs", "change", "help"
            if (SConfig.MenuStartPage.Value == "favs")
                return FavoritesList;
            else if (SConfig.MenuStartPage.Value == "change")
                return SuitsList;
            else if (SConfig.MenuStartPage.Value == "help")
                return HelpPage;
            else
                return HomePage;
        }
    }
}
