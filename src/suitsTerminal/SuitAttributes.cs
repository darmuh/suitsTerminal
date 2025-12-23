using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static OpenLib.Common.CommonStringStuff;
using static suitsTerminal.RackManager;

namespace suitsTerminal;
internal class SuitAttributes : MonoBehaviour
{
    private static string _defaultSuit = string.Empty;
    internal static bool UpdateDefault = true;
    internal SuitMenuItem MenuItem = null!;

    internal UnlockableSuit Suit = null!;
    internal bool HideFromTerminal = false;
    internal bool IsOnRack = false;
    internal string Name = string.Empty;
    internal int ID = -1;

    //extra stuff
    internal bool IsFav = false;

    internal void Reset()
    {
        HideFromTerminal = false;
        IsOnRack = false;
    }

    internal bool IsDefault()
    {
        if (Plugin.SuitSaver)
            return false;

        if (!UpdateDefault)
            return Name.Equals(_defaultSuit, StringComparison.InvariantCultureIgnoreCase);


        if (ModConfig.PersonalizedDefault.Value)
        {
            string FilePath = Plugin.PersonalFilesPath();

            if (!File.Exists(FilePath + @"\defaultSuit.txt"))
                File.WriteAllText(FilePath + @"\defaultSuit.txt", string.Empty);

            _defaultSuit = File.ReadAllText(FilePath + @"\defaultSuit.txt");
            Loggers.LogDebug($"Default Suit from personal files - {_defaultSuit}");
            UpdateDefault = false;
            return Name.Equals(_defaultSuit, StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
            if (ModConfig.DefaultSuit.Value.Length < 1 || ModConfig.DefaultSuit.Value.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return Name.Equals(ModConfig.DefaultSuit.Value, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    internal static void WearDefault()
    {
        UpdateDefault = true;
        Plugin.EquipDefault = false;
        SuitAttributes def = AllSuits.FirstOrDefault(x => x.IsDefault());

        if (def != null)
        {
            Loggers.LogMessage($"Equipping default suit - {def.Name}");
            def.WearSuit();
        }
        else
            Loggers.LogDebug("No default suit detected");
        
    }

    internal bool IsLocked()
    {
        //suits will never be locked
        if (!ModConfig.EnforcePaidSuits.Value)
            return false;

        var UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;

        //purchasing the suit is impossible, so it cannot be locked
        if (UnlockableItems[Suit.syncedSuitID.Value].shopSelectionNode == null)
            return false;

        //this item has not been "spawned" yet and will be considered locked
        if (!UnlockableItems[Suit.syncedSuitID.Value].spawnPrefab)
        {
            Loggers.LogMessage($"Locked suit [{UnlockableItems[Suit.syncedSuitID.Value].unlockableName}] detected, menu item is disabled.");
            Loggers.LogDebug($"hasBeenUnlockedByPlayer: {UnlockableItems[Suit.syncedSuitID.Value].hasBeenUnlockedByPlayer} \nalreadyUnlocked: {UnlockableItems[Suit.syncedSuitID.Value].alreadyUnlocked}");
            return true;
        }

        //suit is not locked
        Loggers.LogDebug($"{Name} is not locked.\n{UnlockableItems[Suit.syncedSuitID.Value].unlockableName}:\nhasBeenUnlockedByPlayer: {UnlockableItems[Suit.syncedSuitID.Value].hasBeenUnlockedByPlayer} \nalreadyUnlocked: {UnlockableItems[Suit.syncedSuitID.Value].alreadyUnlocked}");

        return false;
    }

    internal void Awake()
    {
        UnlockableSuit item = GetComponent<UnlockableSuit>();
        Suit = item;
        ID = item.syncedSuitID.Value;
        Name = GetName(item);
        HideFromTerminal = ShouldHideTerm();

        MenuItem = (SuitMenuItem)Menu.SuitsMenu.AllMenuItemsOfType.FirstOrDefault(x => x.Name == Name);
        MenuItem ??= new(Name, new())
            {
                Header = () => $"========[( {Name} )]========\r\n",
                Footer = Menu.GetFooter,
                OnPageLoad = OnPageLoad
            };
        MenuItem.SuitProps = this; //always associate with latest suitprop
        IsFav = IsFavorite();

        if (HideFromTerminal)
            return;

        Menu.SuitsList.AddNestedItem(MenuItem);

        if (IsFav)
            Menu.FavoritesList.NestedMenus.Add(MenuItem);

        MenuItem.SelectionEvent.RemoveAllListeners();
        MenuItem.SelectionEvent.AddListener(Selection);

        Loggers.LogDebug($"SuitAttributes created for {Name}");
        AllSuits.Add(this);
    }

    private void OnDestroy()
    {
        Plugin.Log.LogDebug($"Destroying SuitAttribute! [{this.Name}]");
        AllSuits.Remove(this);
    }
    
    //Used to display new suit without making the change permanent
    internal void Preview()
    {
        Loggers.LogDebug("Preview");
        if (Menu.CurrentNest == null)
            return;

        if (Menu.SuitsMenu.ActiveSelection >= Menu.CurrentNest.Count)
            return;

        if (Menu.CurrentNest[Menu.SuitsMenu.ActiveSelection] != MenuItem)
            return;

        UnlockableSuit.SwitchSuitForPlayer(Plugin.LocalPlayer, Suit.suitID, false);
    }

    internal void Selection()
    {
        Menu.PotentialSelection = this;
        Menu.PotentialSelection.MenuItem.Parent = Menu.SuitsMenu.CurrentMenuItem; //for fluid exits without having to constantly adjust the menu item parent
    }

    internal void WearSuit()
    {
        CommandHandler.BetterSuitPick(this);
    }

    internal bool IsFavorite()
    {
        RemoveFromFavs(); //remove old entries if they exist
        if (FavsList.Count == 0)
            return false;

        if (FavsList.Any(x => x.Equals(Name, StringComparison.InvariantCultureIgnoreCase)))
        {
            Loggers.LogDebug($"{Name} is detected in favorites list");
            AddToFavs(); //add back (or for the first time) to list here
            return true;
        }

        Loggers.LogDebug($"{Name} is *NOT* a favorite");
        return false;
    }

    internal void ToggleDefault()
    {
        UpdateDefault = true;
        if (IsDefault())
        {
            Plugin.SaveDefault(string.Empty);
        }
        else
        {
            Plugin.SaveDefault(Name);
        }
    }

    internal void ToggleFav()
    {
        if (IsFav)
        {
            RemoveFromFavs();

            Loggers.LogInfo($"{Name} removed from favorites listing");
            if (FavCount < 1 && Menu.FavoritesList.IsActive)
            {
                Plugin.SaveToConfig(FavList, out string configSave);
                Plugin.SaveFavorites(configSave);
                return;
            }
        }
        else
        {
            AddToFavs();
            Loggers.LogInfo($"{Name} added to favorites listing");
        }

        Plugin.SaveToConfig(FavList, out string saveToConfig);
        Plugin.SaveFavorites(saveToConfig);
    }

    internal void RemoveFromFavs()
    {
        IsFav = false;
        Menu.FavoritesList.RemoveChild(MenuItem);
    }

    internal void AddToFavs()
    {
        if (MenuItem == null)
        {
            Loggers.WARNING($"menuItem is null for {Name}");
            return;
        }

        IsFav = true;
        Menu.FavoritesList.AddNestedItem(MenuItem);

        Loggers.LogDebug($"AddToFavs has added {Name}!");
    }

    internal bool ShouldHideTerm()
    {
        List<string> dontAddTerminal = GetListToLower(GetKeywordsPerConfigItem(ModConfig.DontAddToTerminal.Value, ','));

        return dontAddTerminal.Any(x => x.Equals(Name, StringComparison.InvariantCultureIgnoreCase));
    }

    private void DetermineChildren(bool isLocked)
    {
        MenuItem.NestedMenus.Clear();

        if (isLocked)
            MenuItem.NestedMenus.Add(Menu.PurchaseSuitFromStore);
        else
            MenuItem.NestedMenus.Add(Menu.SelectSuit);

        MenuItem.NestedMenus.Add(Menu.FavoriteSuit);
        MenuItem.NestedMenus.Add(Menu.SetDefaultSuit);
        
    }

    internal void OnPageLoad()
    {
        if (Menu.SuitsMenu.CurrentMenuItem == MenuItem)
            return;

        Loggers.LogDebug($"{Name} OnPageLoad, Current - [{Menu.SuitsMenu.CurrentMenuItem.Name}]");
        if(Plugin.LocalPlayer.currentSuitID != ID)
            Preview();
        bool locked = IsLocked();
        MenuItem.Suffix = string.Empty;
        DetermineChildren(locked);

        if (locked)
        {
            MenuItem.Prefix = "<color=#666663>";
            MenuItem.Suffix = "</color>";
        }

        if (IsFav)
            MenuItem.Suffix += " (*)";

        if (CurrentSuit == this)
            MenuItem.Suffix += " [EQUIPPED]";
    }

    internal static string GetName(UnlockableSuit item)
    {
        string SuitName = StartOfRound.Instance.unlockablesList.unlockables[item.syncedSuitID.Value].unlockableName;

        if (AllSuits.FindAll(x => x.Name == SuitName).Count > 0)
            SuitName += $"({item.syncedSuitID.Value})"; //suit with same name exists, adding to name for advanced menu only
        return SuitName;
    }

    internal bool IsCurrent()
    {
        return CurrentSuit == this;
    }

}
