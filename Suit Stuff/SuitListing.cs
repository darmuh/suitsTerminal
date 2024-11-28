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
        internal int CurrentMenu = 0; //1 = favs
        internal List<string> NameList = [];
        internal List<string> FavList = [];

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

        internal void RefreshFavorites(bool checkList = false)
        {
            if (suitListing.SuitsList.Count == 0)
                return;

            if (checkList)
            {
                suitListing.FavList = []; //empty list

                foreach (SuitAttributes fav in SuitsList)
                {
                    fav.FavIndex = -1;
                    fav.IsFav = fav.IsFavorite();
                    fav.SetIndex();
                    Plugin.X($"SuitAttributes updated for {fav.Name}");
                }
            }
            else
            {
                foreach (SuitAttributes fav in SuitsList)
                    fav.RefreshFavIndex();
            }
        }

    }

    internal class SuitAttributes
    {
        internal UnlockableSuit Suit;
        internal bool HideFromTerminal = false;
        internal bool IsOnRack = false;
        internal string Name = "placeholder";
        internal int UniqueID = -1;

        //extra stuff
        internal bool IsFav = false;
        internal int MainMenuIndex = -1;
        internal int FavIndex = -1;
        internal bool currentSuit = false;

        internal SuitAttributes(UnlockableSuit item, List<UnlockableItem> UnlockableItems, ref Dictionary<int, string> suitNameToID)
        {
            Suit = item;
            Name = GetName(item, UnlockableItems, ref suitNameToID);
            UniqueID = item.syncedSuitID.Value;
            suitListing.NameList.Add(Name);
            HideFromTerminal = ShouldHideTerm();
            IsFav = IsFavorite();
            SetIndex();
            Plugin.X($"SuitAttributes created for {Name}");
        }

        internal void UpdateSuit(UnlockableSuit item, List<UnlockableItem> UnlockableItems, ref Dictionary<int, string> suitNameToID)
        {
            Suit = item;
            Name = GetName(item, UnlockableItems, ref suitNameToID);
            UniqueID = item.syncedSuitID.Value;
            suitListing.NameList.Add(Name);
            HideFromTerminal = ShouldHideTerm();
            IsFav = IsFavorite();
            SetIndex();
            Plugin.X($"SuitAttributes updated for {Name}");
        }

        internal void SetIndex()
        {
            if (this.HideFromTerminal)
            {
                this.MainMenuIndex = -1;
                suitListing.NameList.Remove(this.Name);
            }
            this.MainMenuIndex = suitListing.NameList.IndexOf(this.Name);

            if (this.IsFav && !suitListing.FavList.Contains(this.Name))
            {
                this.AddToFavs();
            }

        }

        internal int GetIndex(List<string> listing)
        {
            if (listing.Contains(this.Name))
            {
                return listing.IndexOf(this.Name);
            }
            return -1;
        }

        internal bool IsFavorite()
        {
            if (favsList.Count == 0)
                return false;

            if (favsList.Any(x => x.ToLower() == this.Name.ToLower()))
            {
                Plugin.X($"{this.Name} is detected in favorites list");
                return true;
            }

            Plugin.X($"{this.Name} is *NOT* a favorite");
            return false;
        }

        internal void RemoveFromFavs()
        {
            this.IsFav = false;
            suitListing.FavList.Remove(this.Name);
            this.FavIndex = -1;
        }

        internal void AddToFavs()
        {
            this.IsFav = true;

            if (suitListing.FavList.Contains(this.Name))
            {
                this.FavIndex = suitListing.FavList.IndexOf(this.Name);
                return;
            }
            
            suitListing.FavList.Add(this.Name);
            this.FavIndex = suitListing.FavList.IndexOf(this.Name);
        }

        internal void RefreshFavIndex()
        {
            if (!suitListing.FavList.Contains(this.Name))
                return;

            this.FavIndex = suitListing.FavList.IndexOf(this.Name);
        }

        internal bool ShouldHideTerm()
        {
            List<string> dontAddTerminal = GetListToLower(GetKeywordsPerConfigItem(SConfig.DontAddToTerminal.Value, ','));

            if (dontAddTerminal.Any(x => x.ToLower() == this.Name.ToLower()))
                return true;

            return false;
        }

        internal static string GetName(UnlockableSuit item, List<UnlockableItem> UnlockableItems, ref Dictionary<int, string> suitNameToID)
        {
            string SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;

            if (suitListing.NameList.Contains(SuitName) && SConfig.AdvancedTerminalMenu.Value)
                SuitName = SuitName + $"({item.suitID})"; //suit with same name exists, adding to name for advanced menu only

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
