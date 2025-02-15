using System.Collections.Generic;
using System.Linq;
using static OpenLib.Common.CommonStringStuff;
using static suitsTerminal.AllSuits;
using static suitsTerminal.StringStuff;


namespace suitsTerminal.Suit_Stuff
{
    internal class SuitListing
    {
        internal List<SuitAttributes> SuitsList = [];
        internal List<UnlockableSuit> RawSuitsList = [];
        internal List<string> NameList = [];
        internal List<string> FavList = [];

        internal void ClearAll()
        {
            NameList.Clear();
            SuitsList.ForEach(x => x.Reset());
        }

        internal bool Contains(int query, out SuitAttributes thisSuit)
        {
            foreach (SuitAttributes suit in SuitsList)
            {
                if (suit.UniqueID == query)
                {
                    thisSuit = suit;
                    return true;
                }

            }
            thisSuit = null!;
            return false;
        }

        internal bool Contains(UnlockableSuit query, out SuitAttributes thisSuit)
        {
            foreach (SuitAttributes suit in SuitsList)
            {
                if (suit.Suit == query)
                {
                    thisSuit = suit;
                    return true;
                }

            }

            thisSuit = null!;
            return false;
        }

    }

    internal class SuitAttributes
    {
        //BetterMenu Stuff
        internal SuitMenuItem menuItem;
        internal OpenLib.Events.Events.CustomEvent SelectSuit = new();
        
        internal UnlockableSuit Suit;
        internal bool HideFromTerminal = false;
        internal bool IsOnRack = false;
        internal string Name = "placeholder";
        internal int UniqueID = -1;

        //extra stuff
        internal bool IsFav = false;
        internal bool currentSuit = false;

        internal void Reset()
        {
            HideFromTerminal = false;
            IsOnRack = false;
            currentSuit = false;
            IsFav = false;
        }

        internal SuitAttributes(UnlockableSuit item, List<UnlockableItem> UnlockableItems, ref Dictionary<int, string> suitNameToID)
        {
            Suit = item;
            Name = GetName(item, UnlockableItems, ref suitNameToID);
            UniqueID = item.syncedSuitID.Value;
            suitListing.NameList.Add(Name);
            HideFromTerminal = ShouldHideTerm();

            menuItem = new(Name, SelectSuit);
            menuItem.AddToBetterMenu();
            IsFav = IsFavorite();

            if (HideFromTerminal)
                return;

            menuItem.OnPageLoad = GetSuffix;
            
            menuItem.SetParentMenu(AdvancedMenu.SuitsList);
            
            if (IsFav)
                menuItem.AddNestedItem(AdvancedMenu.FavoritesList);

            SelectSuit.AddListener(WearSuit);

            Plugin.X($"SuitAttributes created for {Name}");
        }

        internal void WearSuit()
        {
            CommandHandler.BetterSuitPick(this);
        }

        internal void UpdateSuit(UnlockableSuit item, List<UnlockableItem> UnlockableItems, ref Dictionary<int, string> suitNameToID)
        {
            Suit = item;
            Name = GetName(item, UnlockableItems, ref suitNameToID);
            UniqueID = item.syncedSuitID.Value;
            suitListing.NameList.Add(Name);
            HideFromTerminal = ShouldHideTerm();
            IsFav = IsFavorite();
            menuItem.AddToBetterMenu();
            menuItem.Name = Name;

            if (!AdvancedMenu.SuitsList.NestedMenus.Contains(menuItem))
            {
                if (!HideFromTerminal)
                {
                    menuItem.SetParentMenu(AdvancedMenu.SuitsList);

                    if (IsFav)
                        menuItem.AddNestedItem(AdvancedMenu.FavoritesList);
                }
            }
            else
            {
                if(HideFromTerminal)
                {
                    menuItem.Parent = null!;
                    AdvancedMenu.SuitsList.NestedMenus.Remove(menuItem);

                    if (AdvancedMenu.FavoritesList.NestedMenus.Contains(menuItem))
                        AdvancedMenu.FavoritesList.NestedMenus.Remove(menuItem);
                }
            }
            Plugin.X($"SuitAttributes updated for {Name}");
        }

        internal bool IsFavorite()
        {
            if (favsList.Count == 0)
                return false;

            if (favsList.Any(x => x.ToLower() == Name.ToLower()))
            {
                Plugin.X($"{Name} is detected in favorites list");
                if(!suitListing.FavList.Contains(Name))
                    suitListing.FavList.Add(Name);
                return true;
            }

            if(suitListing.FavList.Contains(Name))
                suitListing.FavList.Remove(Name);
            Plugin.X($"{Name} is *NOT* a favorite");
            return false;
        }

        internal void RemoveFromFavs()
        {
            IsFav = false;
            if (!suitListing.FavList.Contains(Name))
                return;
            suitListing.FavList.Remove(Name);
            AdvancedMenu.FavoritesList.NestedMenus.Remove(menuItem);
        }

        internal void AddToFavs()
        {
            if (SConfig.AdvancedTerminalMenu.Value && menuItem == null)
            {
                Plugin.WARNING($"menuItem is null for {Name}");
                return;
            }

            IsFav = true;

            if (suitListing.FavList.Contains(Name))
                return;
            
            suitListing.FavList.Add(Name);
            if(!AdvancedMenu.FavoritesList.NestedMenus.Contains(menuItem))
                AdvancedMenu.FavoritesList.NestedMenus.Add(menuItem);

            Plugin.X($"AddToFavs has added {Name}!");
        }

        internal bool ShouldHideTerm()
        {
            List<string> dontAddTerminal = GetListToLower(GetKeywordsPerConfigItem(SConfig.DontAddToTerminal.Value, ','));

            if (dontAddTerminal.Any(x => x.ToLower() == Name.ToLower()))
                return true;

            return false;
        }

        internal void GetSuffix()
        {
            menuItem.Suffix = string.Empty;

            if (IsFav)
                menuItem.Suffix += " (*)";

            if (currentSuit)
                menuItem.Suffix += " [EQUIPPED]";
        }

        internal static string GetName(UnlockableSuit item, List<UnlockableItem> UnlockableItems, ref Dictionary<int, string> suitNameToID)
        {
            string SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;

            if (suitListing.NameList.Any(s => s.ToLower() == SuitName.ToLower()) && SConfig.AdvancedTerminalMenu.Value)
                SuitName += $"({item.syncedSuitID.Value})"; //suit with same name exists, adding to name for advanced menu only

            if (!SConfig.AdvancedTerminalMenu.Value)
                SuitName = TerminalFriendlyString(SuitName);
            if (!suitNameToID.ContainsKey(item.syncedSuitID.Value))
                suitNameToID.Add(item.syncedSuitID.Value, SuitName);
            else
                Plugin.WARNING($"WARNING: duplicate suitID detected: {item.syncedSuitID.Value}\n{SuitName} will not be added to listing");

            return SuitName;
        }

    }
}
